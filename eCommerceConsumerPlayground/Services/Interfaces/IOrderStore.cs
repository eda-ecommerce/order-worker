using ECommerceConsumerPlayground.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IOrderStore
{
    Task SavePaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task UpdateOrderAsync(Order order);
    Task SaveDataAsync(Order order);
    Task<bool> CheckIfPaymentEntryAlreadyExistsAsync(Payment payment);
    Task<bool> CheckIfOrderExistsAsync(Payment payment);
    Task<bool> CheckIfEntryAlreadyExistsAsync(Order order);
}