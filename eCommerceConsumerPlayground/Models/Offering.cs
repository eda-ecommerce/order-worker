using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eCommerceConsumerPlayground.Models.Database;

public class Offering
{
    public Guid OfferingId { get; set; }

    public Guid ProductId { get; set; }
    
    public int Quantity { get; set; }

    public float Price { get; set; }

    public bool Status { get; set; }
    
    public Guid ItemId { get; set; }
    
    public Item Items { get; set; }
}