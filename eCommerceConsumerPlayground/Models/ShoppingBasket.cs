using eCommerceConsumerPlayground.Models.Database;

namespace ECommerceConsumerPlayground.Models;

public class ShoppingBasket
{
    public Guid shoppingBasketId { get; set; }
    
    public Guid customerId { get; set; }
    
    public float totalPrice { get; set; }
    
    public int totalItemQuantity { get; set; }
     
     public List<Item> shoppingBasketItems { get; set; } = new List<Item>();

}
