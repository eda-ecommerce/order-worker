using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;

namespace paymentWorker.Models;

public class KafkaSchemaOrder
{
    public Guid OrderId { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public DateOnly OrderDate { get; set; }
    
    public OrderStatus OrderStatus { get; set; }
    
    public float TotalPrice { get; set; }
     
    public ICollection<KafkaSchemaItem> Items { get; set; } = new List<KafkaSchemaItem>();
}