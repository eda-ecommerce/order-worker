using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerceConsumerPlayground.Services;

/// <summary>
/// Implementation of User objects in database
/// </summary>
public class OrderStore : IOrderStore
{
    private readonly ILogger<OrderStore> _logger;
    private readonly AppDbContext _context;

    public OrderStore(ILogger<OrderStore> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task UpdateOrderAsync(Order order)
    {
        _logger.LogInformation($"Starting update operations for order object '{order}' in database.");
        try
        {
            // Check if entry already exists
            var orderr = await _context.Orders.FirstOrDefaultAsync(u => u.OrderId == order.OrderId);
            
            // If not already exists, than persist
            orderr.OrderStatus = OrderStatus.Paid;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Order object '{order}' could not be saved on database. Message: {e.Message}");
        }
    }
    
    public async Task SaveOrderAsync(Order order)
    {
        _logger.LogInformation($"Starting persistence operations for order object '{order}' in database.");
        try
        {
            // Check if entry already exists
            var orderExists = await CheckIfEntryAlreadyExistsAsync(order);
            if (orderExists)
            {
                _logger.LogInformation($"Order object '{order.OrderId}' already exists in database. No new persistence.");
                return;
            }



            // If not already exists, than persist
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Order object '{order}' could not be saved on database. Message: {e.Message}");
        }
    }
    
    public async Task<bool> CheckIfOrderExistsAsync(Payment payment)
    {
        var orderExists = await _context.Orders.AnyAsync(u => u.OrderId == payment.OrderId);
        return orderExists;
    }

    public async Task<bool> CheckIfEntryAlreadyExistsAsync(Order order)
    {
        var orderExists = await _context.Orders.AnyAsync(u => u.OrderId == order.OrderId);
        return orderExists;
    }
}