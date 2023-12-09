using System.ComponentModel.DataAnnotations;
using eCommerceConsumerPlayground.Models.Database;

namespace ECommerceConsumerPlayground.Models;

public class Order
{
    [Key]
    public Guid OrderId { get; set; }

    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public string OrderStatus { get; set; }

    /// <summary>
    /// Bsp: DieterMücke
    /// </summary>
    public Double TotalPrice { get; set; }
    
    /// <summary>
    /// Bsp: DieterMücke
    /// </summary>
     public List<Item> Items { get; set; }
    
}
