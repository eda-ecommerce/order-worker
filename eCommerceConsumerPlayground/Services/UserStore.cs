using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerceConsumerPlayground.Services;

/// <summary>
/// Implementation of User objects in database
/// </summary>
public class UserStore : IUserStore
{
    private readonly ILogger<UserStore> _logger;
    private readonly AppDbContext _context;

    public UserStore(ILogger<UserStore> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task SaveDataAsync(Order order)
    {
        _logger.LogInformation($"Starting persistence operations for user object '{order}' in database.");
        try
        {
            // Check if entry already exists
            var orderExists = await CheckIfEntryAlreadyExistsAsync(order);
            if (orderExists)
            {
                _logger.LogInformation($"User object '{order.OrderId}' already exists in database. No new persistence.");
                return;
            }



            // If not already exists, than persist
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
        }
        catch (Exception e)
        {
            _logger.LogError($"User object '{order}' could not be saved on database. Message: {e.Message}");
        }
    }

    private async Task<bool> CheckIfEntryAlreadyExistsAsync(Order order)
    {
        var orderExists = await _context.Orders.AnyAsync(u => u.OrderId == order.OrderId);
        return orderExists;
    }
}