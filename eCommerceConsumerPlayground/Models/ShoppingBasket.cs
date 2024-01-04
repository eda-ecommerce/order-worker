using eCommerceConsumerPlayground.Models.Database;

namespace ECommerceConsumerPlayground.Models;

public class ShoppingBasket
{
    public Guid ShoppingBasketId { get; set; }
    
    public Guid CustomerId { get; set; }
     
     public List<Item> Items { get; set; } = new List<Item>();

}
