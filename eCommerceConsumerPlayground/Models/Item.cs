using System.ComponentModel.DataAnnotations;
using ECommerceConsumerPlayground.Models;

namespace eCommerceConsumerPlayground.Models.Database;

public class Item
{
    public Guid ItemId { get; set; }
    
    public int Quantity { get; set; }
    
    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Offering Offering { get; set; }
}