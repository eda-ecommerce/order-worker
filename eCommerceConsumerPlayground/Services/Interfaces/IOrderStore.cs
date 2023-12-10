using ECommerceConsumerPlayground.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IOrderStore
{
    Task SaveDataAsync(Order order);
    Task<bool> CheckIfEntryAlreadyExistsAsync(Order order);
}