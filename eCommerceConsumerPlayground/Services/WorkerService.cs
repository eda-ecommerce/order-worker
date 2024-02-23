using System.Text.Json;
using ECommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
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
        KAFKA_BROKER = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKABROKER")) ? Environment.GetEnvironmentVariable("KAFKABROKER") : _configuration.GetValue<string>("Kafka:Broker");
        KAFKA_TOPIC1 = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKATOPIC1")) ? Environment.GetEnvironmentVariable("KAFKATOPIC1") : _configuration.GetValue<string>("Kafka:Topic1");
        KAFKA_TOPIC2 = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKATOPIC2")) ? Environment.GetEnvironmentVariable("KAFKATOPIC2") : _configuration.GetValue<string>("Kafka:Topic2");
        KAFKA_TOPIC3 = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKATOPIC2")) ? Environment.GetEnvironmentVariable("KAFKATOPIC3") : _configuration.GetValue<string>("Kafka:Topic3");
        KAFKA_GROUPID = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("KAFKAGROUPID")) ? Environment.GetEnvironmentVariable("KAFKAGROUPID") : _configuration.GetValue<string>("Kafka:GroupId");

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
            KAFKA_TOPIC3
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
                    
                    // Handle message...
                    var shoppingBasket = JsonSerializer.Deserialize<KafkaSchemaShoppingBasket>(consumeResult.Message.Value)!;
                    var payment = JsonSerializer.Deserialize<KafkaSchemaPayment>(consumeResult.Message.Value)!;
                   
                    var paymentSource = Encoding.UTF8.GetString(consumeResult.Message.Headers.GetLastBytes("source"), 0, consumeResult.Message.Headers.GetLastBytes("source").Length);
                    
                    var paymentOperation = Encoding.UTF8.GetString(consumeResult.Message.Headers.GetLastBytes("operation"), 0, consumeResult.Message.Headers.GetLastBytes("operation").Length);
                    
                    Order findOrder = null;
                    if(paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && payment.OrderId != null)
                    {
                        findOrder = await _orderStore.GetOrderAsync(payment.OrderId);
                    }
                    
                    List<KafkaSchemaItem> shoppingBasketItems = new List<KafkaSchemaItem>();
                    if ((paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null))
                    {
                        findOrder.Items.ToList().ForEach(s =>
                        {
                            shoppingBasketItems.Add(
                                new()
                                {
                                    shoppingBasketItemId = s.shoppingBasketId,
                                    shoppingBasketId = s.shoppingBasketId,
                                    itemState = s.itemState,
                                    quantity = s.quantity,
                                    offeringId = s.offeringId,
                                    totalPrice = s.totalPrice
                                });
                        });
                    }
                    else
                    {
                        shoppingBasket.shoppingBasketItems.ForEach(s =>
                        {
                            shoppingBasketItems.Add(
                                new()
                                {
                                    shoppingBasketItemId = s.shoppingBasketItemId,
                                    shoppingBasketId = s.shoppingBasketId,
                                    itemState = s.itemState,
                                    quantity = s.quantity,
                                    offeringId = s.offeringId,
                                    totalPrice = s.totalPrice
                                });
                        });
                    }
                    
                    List<Item> shoppingBasketItemss = new List<Item>();
                    if ((paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null))
                    {
                        findOrder.Items.ToList().ForEach(s =>
                        {
                            shoppingBasketItemss.Add(
                                new()
                                {
                                    shoppingBasketItemId = s.shoppingBasketItemId,
                                    shoppingBasketId = s.shoppingBasketId,
                                    itemState = s.itemState,
                                    quantity = s.quantity,
                                    offeringId = s.offeringId,
                                    totalPrice = s.totalPrice
                                });
                        });
                    }
                    else
                    {
                        shoppingBasket.shoppingBasketItems.ForEach(s =>
                        {
                            shoppingBasketItemss.Add(
                                new()
                                {
                                    shoppingBasketItemId = s.shoppingBasketItemId,
                                    shoppingBasketId = s.shoppingBasketId,
                                    itemState = s.itemState,
                                    quantity = s.quantity,
                                    offeringId = s.offeringId,
                                    totalPrice = s.totalPrice
                                });
                        });
                    }

                    var orderId = Guid.NewGuid();
                    var kafkaSchemaOrder = new KafkaSchemaOrder()
                    {
                        OrderId = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated") ? payment.OrderId : orderId,
                        CustomerId = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.CustomerId : shoppingBasket.customerId,
                        OrderDate = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.OrderDate : DateOnly.FromDateTime(DateTime.Now),
                        OrderStatus = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated") ? OrderStatus.Paid.ToString() : OrderStatus.InProcess.ToString(),
                        TotalPrice = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.TotalPrice : shoppingBasket.totalPrice,
                        Items = shoppingBasketItems
                    };
                    
                    var order = new Order()
                        {
                            OrderId = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated") ? payment.OrderId : orderId,
                            CustomerId = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.CustomerId : shoppingBasket.customerId,
                            OrderDate = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.OrderDate : DateOnly.FromDateTime(DateTime.Now),
                            OrderStatus = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated") ? OrderStatus.Paid : OrderStatus.InProcess,
                            TotalPrice = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && findOrder != null) ? findOrder.TotalPrice : shoppingBasket.totalPrice,
                            Items = shoppingBasketItemss
                        };
                        

                        var kafkaOrderHeader = new KafkaSchemaOrderHeader()
                        {
                            source = "order",
                            timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                            operation = (paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated") ? "updated" : "created",
                        };
                        
                        // Produce messages
                        ProducerConfig configProducer = new ProducerConfig
                        {
                            BootstrapServers = KAFKA_BROKER,
                            ClientId = Dns.GetHostName()
                        };

                        using var producer = new ProducerBuilder<Null, string>(configProducer).Build();

                        var header = new Headers();
                        header.Add("source", Encoding.UTF8.GetBytes(kafkaOrderHeader.source));
                        header.Add("timestamp", Encoding.UTF8.GetBytes(kafkaOrderHeader.timestamp.ToString()));
                        header.Add("operation", Encoding.UTF8.GetBytes(kafkaOrderHeader.operation));

                            Console.WriteLine("I am before here");

                        
                        // if statment is required so that a message is only produced if an order does not yet exist.
                        if (!await _orderStore.CheckIfEntryAlreadyExistsAsync(order))
                        {
                            Console.WriteLine("I am here");
                            Console.WriteLine(paymentSource);
                            Console.WriteLine(paymentOperation);

                            if (paymentSource == KAFKA_TOPIC1 && paymentOperation == "CHECKOUT") {
                                                        Console.WriteLine("I am uhkgvchhere");

                            }
                            
                            if (paymentSource == KAFKA_TOPIC1 && paymentOperation == "CHECKOUT" && !await _orderStore.CheckIfShoppingBasketIdExistsAsync(shoppingBasket.shoppingBasketId))
                            {
                               Console.WriteLine("I am super here");

                                await producer.ProduceAsync(KAFKA_TOPIC2, new Message<Null, string>
                                { 
                                    Value = JsonSerializer.Serialize<KafkaSchemaOrder>(kafkaSchemaOrder), 
                                    Headers = header
                                });
                                // Persistence
                                await _orderStore.SaveOrderAsync(order);
                            }
                        }
                        
                    if(paymentSource == KAFKA_TOPIC3 && paymentOperation == "updated" && await _orderStore.CheckIfOrderExistsAsync(payment))
                    {
                        await producer.ProduceAsync(KAFKA_TOPIC2, new Message<Null, string>
                        {
                            Value = JsonSerializer.Serialize<KafkaSchemaOrder>(kafkaSchemaOrder),
                            Headers = header
                        });
                        await _orderStore.UpdateOrderAsync(order);
                    }

                }
                catch (ConsumeException  e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.LogWarning($"Error on consuming Kafka Message. Reason: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        _logger.LogError("Fatal error on consuming Kafka Message..");
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

    public void CloseConsumer()
    {
        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        _logger.LogWarning(2001, "Closing Kafka (unsubscribe and close events)..");
        _kafkaConsumer.Unsubscribe();
        _kafkaConsumer.Close();
    }
    
    public (bool, ShoppingBasket?) DeserializeKafkaMessage(ConsumeResult<Ignore, string> consumeResult)
    {
        ShoppingBasket? shoppingBasket = null;
        try
        {
            shoppingBasket = JsonSerializer.Deserialize<ShoppingBasket>(consumeResult.Message.Value);
            return (true, shoppingBasket);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(205, "Message could not be deserialized into the default model.");
            _logger.LogWarning(205, $"Exception: {ex.Message}");
            return (false, null);
        }
    }
}
