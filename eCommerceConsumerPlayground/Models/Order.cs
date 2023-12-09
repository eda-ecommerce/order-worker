using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
