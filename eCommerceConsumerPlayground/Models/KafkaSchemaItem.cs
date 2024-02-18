namespace eCommerceConsumerPlayground.Models;

public class KafkaSchemaItem
{
    public Guid shoppingBasketId { get; set; }
    
    public int quantity { get; set; }

    public Guid offeringId { get; set; }
    
    public float totalPrice { get; set; }
    
    public string itemState { get; set; }
}