using eCommerceConsumerPlayground.Models;
using eCommerceConsumerPlayground.Models.Database;

namespace ECommerceConsumerPlayground.Models;

public class Order
{
    public Guid OrderId { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public DateOnly OrderDate { get; set; }
    
    public OrderStatus OrderStatus { get; set; }
    
    public float TotalPrice { get; set; }
    
    public Payment? Payment { get; set; }
     
     public ICollection<Item> Items { get; set; } = new List<Item>();

}
