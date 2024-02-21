using System.ComponentModel.DataAnnotations;
using ECommerceConsumerPlayground.Models;

namespace eCommerceConsumerPlayground.Models.Database;

public class Item
{
    [Key]
    public Guid shoppingBasketItemId { get; set; }
    
    public Guid shoppingBasketId { get; set; }
    
    public int quantity { get; set; }
    
    public Guid orderId { get; set; }
    
    // public Order order { get; set; }

    public Guid offeringId { get; set; }
    
    public float totalPrice { get; set; }
    
    public string itemState { get; set; }
}