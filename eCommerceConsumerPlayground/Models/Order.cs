using System.ComponentModel.DataAnnotations;
using eCommerceConsumerPlayground.Models.Database;

namespace ECommerceConsumerPlayground.Models;

public class Order
{
    public Guid OrderId { get; set; }
    
    public DateOnly OrderDate { get; set; }
    
    public string OrderStatus { get; set; }
    
    public float TotalPrice { get; set; }
     
     public ICollection<Item> Items { get; set; } = new List<Item>();

}
