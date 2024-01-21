using ECommerceConsumerPlayground.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IOrderStore
{
    Task UpdateOrderAsync(Order order);
    Task SaveOrderAsync(Order order);
   
    Task<bool> CheckIfOrderExistsAsync(Payment payment);
    Task<bool> CheckIfEntryAlreadyExistsAsync(Order order);
}