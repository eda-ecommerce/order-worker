using ECommerceConsumerPlayground.Models;
using paymentWorker.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IOrderStore
{
    Task UpdateOrderAsync(Order order);
    Task SaveOrderAsync(Order order);

    Task<Order> GetOrderAsync(Guid OrderId);
   
    Task<bool> CheckIfOrderExistsAsync(KafkaSchemaPayment payment);
    Task<bool> CheckIfEntryAlreadyExistsAsync(Order order);
    
    Task<bool> CheckIfShoppingBasketIdExistsAsync(Guid shoppingBasketId);
}