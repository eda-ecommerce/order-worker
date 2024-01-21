using ECommerceConsumerPlayground.Models;

namespace eCommerceConsumerPlayground.Models.Database;

public class Item
{
    public Guid ItemId { get; set; }
    
    public int Quantity { get; set; }
    
    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Guid OfferingId { get; set; }
    
    public float TotalPrice { get; set; }
}