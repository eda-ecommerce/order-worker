using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;
using ECommerceConsumerPlayground.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrderWorkerTest;

public class OrderStoreIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrderStoreIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    /*public async Task SaveOrderAndGetOrder_ReturnsSavedOrder()
    {
        // Arrange
        var client = _factory.CreateClient();
        var dbContext = CreateDbContext();

        var order1Id = Guid.NewGuid();
        ICollection<Item> shoppingBasketItems = new List<Item>();
        shoppingBasketItems.Add(new Item()
            {
                ItemId = Guid.NewGuid(),
                OfferingId = Guid.NewGuid(),
                Quantity = 5,
                TotalPrice = 500,
                OrderId = order1Id
            }
        );
        
        // Arrange
        var order1CustomerId = Guid.NewGuid();
        var order1OrderDate = DateOnly.FromDateTime(DateTime.Now);
        var order1Status = OrderStatus.InProcess;
        var order1TotalPrice = 500;
        var order1Items = shoppingBasketItems;
        
        var order = new Order()
        {
            OrderId = order1Id,
            CustomerId = order1CustomerId,
            OrderDate = order1OrderDate,
            OrderStatus = order1Status,
            TotalPrice = order1TotalPrice,
            Items = order1Items
        };
            
        var orderStore = new OrderStore(CreateLogger<OrderStore>(), dbContext);

        // Act
        await orderStore.SaveOrderAsync(order);
        var savedOrder = await orderStore.GetOrderAsync(order.OrderId);

        // Assert
        Assert.NotNull(savedOrder);
        Assert.Equal(order.OrderId, savedOrder.OrderId);

        // Clean up
        dbContext.Dispose();
    } */

    private AppDbContext CreateDbContext()
    {
        var optins = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new AppDbContext(optins);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    private ILogger<T> CreateLogger<T>()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();
        var logger = factory.CreateLogger<T>();
        return logger;
    }
}