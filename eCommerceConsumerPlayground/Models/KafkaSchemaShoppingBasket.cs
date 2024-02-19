using ECommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;

namespace paymentWorker.Models;

public class KafkaSchemaShoppingBasket
{
    public Guid shoppingBasketId { get; set; }
    
    public Guid customerId { get; set; }
    
    public float totalPrice { get; set; }
    
    public int totalItemQuantity { get; set; }
     
    public List<Item> shoppingBasketItems { get; set; } = new List<Item>();
}