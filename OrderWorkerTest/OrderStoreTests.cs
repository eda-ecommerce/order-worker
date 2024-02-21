using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;
using ECommerceConsumerPlayground.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace OrderWorkerTest;

public class OrderStoreTests
{
    private readonly OrderStore _sut;
    private readonly Mock<ILogger<OrderStore>> _mockLogger = new Mock<ILogger<OrderStore>>();

    private async Task<AppDbContext> GetDatabaseContext()
    {
        var optins = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new AppDbContext(optins);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    public OrderStoreTests()
    {
        _sut = new OrderStore(_mockLogger.Object, GetDatabaseContext().Result);
    }
    
    [Fact]
    public async Task SaveDataAsync_ShouldLogCorrectMessage_WhenDataIsStoredInTheDatabase()
    {
        var order1Id = Guid.NewGuid();
        ICollection<Item> shoppingBasketItems = new List<Item>();
        shoppingBasketItems.Add(new Item()
            {
                shoppingBasketItemId = Guid.NewGuid(),
                offeringId = Guid.NewGuid(),
                shoppingBasketId = Guid.NewGuid(),
                itemState = "active",
                quantity = 5,
                totalPrice = 500,
                orderId = order1Id
                
            }
        );
        
        // Arrange
        var order1CustomerId = Guid.NewGuid();
        var order1OrderDate = DateOnly.FromDateTime(DateTime.Now);
        var order1Status = OrderStatus.InProcess;
        var order1TotalPrice = 500;
        var order1Items = shoppingBasketItems;
        
        var order1 = new Order()
        {
            OrderId = order1Id,
            CustomerId = order1CustomerId,
            OrderDate = order1OrderDate,
            OrderStatus = order1Status,
            TotalPrice = order1TotalPrice,
            Items = order1Items
        };
        
        // Act
        _sut.SaveOrderAsync(order1);

        // Assert
        _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $@"Order with Id: '{order1.OrderId}' successfully saved in database.
                OrderId: {order1.OrderId}
                OrderDate: {order1.OrderDate}
                OrderStatus: {order1.OrderStatus}" 
                                                        && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
    [Fact]
    public async Task SaveDataAsync_ShouldLogCorrectMessage_WhenDataAlreadyExistsInDatabase()
    {
        var order1Id = Guid.NewGuid();
        List<Item> shoppingBasketItems = new List<Item>()
        {
            new Item()
            {
                shoppingBasketItemId = Guid.NewGuid(),
                offeringId = Guid.NewGuid(),
                orderId = order1Id,
                quantity = 5,
                totalPrice = 500
            }
        };
        
        // Arrange
        var order1CustomerId = Guid.NewGuid();
        var order1OrderDate = DateOnly.FromDateTime(DateTime.Now);
        var order1Status = OrderStatus.InProcess;
        var order1TotalPrice = 500;
        var order1Items = shoppingBasketItems;
        
        var order1 = new Order()
        {
            OrderId = order1Id,
            CustomerId = order1CustomerId,
            OrderDate = order1OrderDate,
            OrderStatus = order1Status,
            TotalPrice = order1TotalPrice,
            Items = order1Items
        };
        
        // Act
        _sut.SaveOrderAsync(order1);
        _sut.SaveOrderAsync(order1);

        // Assert
        _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"Order object '{order1.OrderId}' already exists in database. No new persistence." 
                                                        && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}