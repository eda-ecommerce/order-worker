using System.Text.Json;
using ECommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using eCommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;
using paymentWorker.Models;

namespace ECommerceConsumerPlayground.Services;

/// <summary>
/// Implementation of Kafka Consumer Service
/// </summary>
public class WorkerService : IWorkerService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IConsumer<Ignore, string> _kafkaConsumer;
    private readonly IConfiguration _configuration;
    private readonly IOrderStore _orderStore;
    private readonly string KAFKA_BROKER;
    private readonly string KAFKA_TOPIC1;
    private readonly string KAFKA_GROUPID;
    private readonly string KAFKA_TOPIC2;
    private readonly string KAFKA_TOPIC3;

    public WorkerService(ILogger<WorkerService> logger, IConfiguration configuration, IOrderStore orderStore)
    {
        _configuration = configuration;
        // Get appsettings and set as static variable
        KAFKA_BROKER = _configuration.GetValue<string>("Kafka:Broker")!;
        KAFKA_TOPIC1 = _configuration.GetValue<string>("Kafka:Topic1")!;
        KAFKA_TOPIC2 = _configuration.GetValue<string>("Kafka:Topic2")!;
        KAFKA_TOPIC3 = _configuration.GetValue<string>("Kafka:Topic3")!;
        KAFKA_GROUPID = _configuration.GetValue<string>("Kafka:GroupId")!;

        //KAFKA_BROKER = "localhost:29092";
        //KAFKA_GROUPID = "ecommerce-gp";
        //KAFKA_TOPIC1 = "test-1";
        // KAFKA_TOPIC2 = "order";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = KAFKA_BROKER,
            GroupId = KAFKA_GROUPID,
            AutoOffsetReset = AutoOffsetReset.Earliest, // if Earliest - begins at offset 0 | if Latest - begins at now
            EnableAutoOffsetStore = false // if false - always begins at offset 0
        };
        
        _logger = logger;
        _kafkaConsumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _orderStore = orderStore;
    }


    public async Task ConsumerLoopAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Subscribe(new string[]
        {
            KAFKA_TOPIC1,
            
        });
        
        // Try and catch to ensure the consumer leaves the group cleanly and final offsets are committed.
        try
        {
            _logger.LogInformation("Loop");
            // Endless consume loop until interrupt
            while (true)
            {
                try
                {
                    var consumeResult = _kafkaConsumer.Consume(cancellationToken);
//                    
                    // Handle message...
                    var shoppingBasket = JsonSerializer.Deserialize<KafkaSchemaShoppingBasket>(consumeResult.Message.Value)!;
                    
                    var order = new Order()
                    {
                        OrderId = Guid.NewGuid(),
                        CustomerId = shoppingBasket.ShoppingBasket.CustomerId,
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderStatus = OrderStatus.InProgress,
                        TotalPrice = CalculateTotalPrice(shoppingBasket.ShoppingBasket.Items),
                        Items = shoppingBasket.ShoppingBasket.Items
                    };

                    var kafkaOrder = new KafkaSchemaOrder()
                    {
                        Source = "Order-Service",
                        Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        Operation = "created",
                        Order = order
                    };
                    
                    // if statment is required so that a message is only produced if an order does not yet exist.
                    if (!await _orderStore.CheckIfEntryAlreadyExistsAsync(order))
                    {
                        // Produce messages
                        ProducerConfig configProducer = new ProducerConfig
                        {
                            BootstrapServers = KAFKA_BROKER,
                            ClientId = Dns.GetHostName()
                        };

                        using var producer = new ProducerBuilder<Null, string>(configProducer).Build();

                        var result = await producer.ProduceAsync(KAFKA_TOPIC2, new Message<Null, string>
                        {
                            Value = JsonSerializer.Serialize<KafkaSchemaOrder>(kafkaOrder)
                        });
                        
                        // Persistence
                        await _orderStore.SaveDataAsync(order);
                        
                        var payment = new Payment()
                        {
                            PaymentId = Guid.NewGuid(),
                            CreatedDate = order.OrderDate,
                            PaymentDate = null,
                            OrderId = order.OrderId,
                            Status = PaymentStatus.Unpaid
                        };

                        var kafkaPayment = new KafkaSchemaPayment()
                        {
                            Source = "Payment-Service",
                            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                            Operation = "created",
                            Payment = payment
                        };
                        
                        var paymentResult = await producer.ProduceAsync(KAFKA_TOPIC3, new Message<Null, string>
                        {
                            Value = JsonSerializer.Serialize<KafkaSchemaPayment>(kafkaPayment)
                        });

                        await _orderStore.SavePaymentAsync(payment);
                    }


                    
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.LogWarning(2000, $"Error on consuming Kafka Message. Reason: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        _logger.LogError(3000, "Fatal error on consuming Kafka Message..");
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Unsubscribe and close
            CloseConsumer();
        }
    }
    
    static float CalculateTotalPrice(List<Item> items)
    {
        float totalPrice = 0;

        foreach (var item in items)
        {
            totalPrice += item.Quantity * (item.Offering.Quantity * item.Offering.Price);
        }

        return totalPrice;
    }

    public void CloseConsumer()
    {
        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        _logger.LogWarning(2001, "Closing Kafka (unsubscribe and close events)..");
        _kafkaConsumer.Unsubscribe();
        _kafkaConsumer.Close();
    }
}