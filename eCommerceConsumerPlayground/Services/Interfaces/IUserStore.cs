using ECommerceConsumerPlayground.Models;

namespace ECommerceConsumerPlayground.Services.Interfaces;

/// <summary>
/// Interface for User objects in database
/// </summary>
public interface IUserStore
{
    Task SaveDataAsync(Order user);
}