using Confluent.Kafka;
using ECommerceConsumerPlayground.Services;
using ECommerceConsumerPlayground.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace OrderWorkerTest;

public class OrderServiceTest
{
    private readonly WorkerService _sut;
    private readonly Mock<ILogger<WorkerService>> _mockLogger = new Mock<ILogger<WorkerService>>();
    private readonly Mock<IOrderStore> _mockOrderStore = new Mock<IOrderStore>();
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(
            new Dictionary<string, string> {
                {"Kafka:Broker", "placeholder"},
                {"Kafka:Topic", "placeholder"},
                {"Kafka:GroupId", "placeholder"},
            }!)
        .Build();
    
    public OrderServiceTest()
    {
        _sut = new WorkerService(_mockLogger.Object, _configuration, _mockOrderStore.Object);
    }

    [Fact]
    public async Task DeserializeKafkaMessage_ShouldReturnValidModel_WhenMessageModelIsCorrect()
    {
        // Arrage
        var shoppingBasketId = "25428531-3d60-4ba6-9409-383ed99b3e9d";
        
        var kafkaMessage = new ConsumeResult<Ignore, string>
        {
            Message = new Message<Ignore, string>
            {
                Value = "{\"ShoppingBasketId\":\"25428531-3d60-4ba6-9409-383ed99b3e9d\",\"CustomerId\":\"42196569-8808-4d8d-a358-8351b6e5d500\",\"TotalItemQuantity\":5,\"TotalPrice\":500,\"Items\":[{\"OfferingId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"TotalPrice\":100,\"Quantity\":1},{\"OfferingId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"TotalPrice\":100,\"Quantity\":1},{\"OfferingId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"TotalPrice\":100,\"Quantity\":1},{\"OfferingId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"TotalPrice\":100,\"Quantity\":1},{\"OfferingId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"TotalPrice\":100,\"Quantity\":1}]}\n"
            }
        };
        
        // Act
        var (isValid, deserializedOrder) = _sut.DeserializeKafkaMessage(kafkaMessage);
        
        // Assert
        isValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeserializeKafkaMessage_ShouldReturnInvalidModel_WhenMessageModelIsNotCorrect()
    {
        // Arrage
        var shoppingBasketId = "25428531-3d60-4ba6-9409-383ed99b3e9d";
        
        var kafkaMessage = new ConsumeResult<Ignore, string>
        {
            Message = new Message<Ignore, string>
            {
                Value = "{\"shoppingBasketId\":\"123123\",\"customerId\":\"123123\",\"totalItemQuantity\":5,\"items\":[{\"totalPrice\":100,\"quantity\":1},{\"offeringId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"totalPrice\":100,\"quantity\":1},{\"offeringId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"totalPrice\":100,\"quantity\":1},{\"offeringId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"totalPrice\":100,\"quantity\":1},{\"offeringId\":\"ab64cc0d-5a0b-4643-93e7-7912e6d6f78f\",\"totalPrice\":100,\"quantity\":1}]}\n"
            }
        };
        
        // Act
        var (isValid, deserializedOrderShoppingBasket) = _sut.DeserializeKafkaMessage(kafkaMessage);
        
        // Assert
        isValid.Should().BeFalse();
    }
}