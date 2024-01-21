using ECommerceConsumerPlayground.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IOrderStore
{
    Task UpdateOrderAsync(Order order);
    Task SaveOrderAsync(Order order);

    Task<Order> GetOrderAsync(Guid OrderId);
   
    Task<bool> CheckIfOrderExistsAsync(Payment payment);
    Task<bool> CheckIfEntryAlreadyExistsAsync(Order order);
}