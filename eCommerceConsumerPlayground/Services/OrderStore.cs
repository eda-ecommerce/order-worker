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
    
    public async Task UpdatePaymentAsync(Payment payment)
    {
        _logger.LogInformation($"Starting update operations for payment object '{payment}' in database.");
        try
        {
            // Check if entry already exists
            var order = await _context.Payments.FirstOrDefaultAsync(u => u.OrderId == payment.OrderId);
            
            // If not already exists, than persist
            order.PaymentDate = payment.PaymentDate;
            order.Status = PaymentStatus.Paid;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Payment object '{payment}' could not be saved on database. Message: {e.Message}");
        }
    }
    
    public async Task SavePaymentAsync(Payment payment)
    {
        _logger.LogInformation($"Starting persistence operations for payment object '{payment}' in database.");
        try
        {
            // Check if entry already exists
            var paymentExists = await CheckIfPaymentEntryAlreadyExistsAsync(payment);
            if (paymentExists)
            {
                _logger.LogInformation($"Payment object '{payment.PaymentId}' already exists in database. No new persistence.");
                return;
            }



            // If not already exists, than persist
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Payment object '{payment}' could not be saved on database. Message: {e.Message}");
        }
    }
    
    public async Task SaveDataAsync(Order order)
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
    
    public async Task<bool> CheckIfPaymentEntryAlreadyExistsAsync(Payment payment)
    {
        var paymentExists = await _context.Payments.AnyAsync(u => u.PaymentId == payment.PaymentId);
        return paymentExists;
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