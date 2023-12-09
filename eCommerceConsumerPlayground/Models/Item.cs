using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eCommerceConsumerPlayground.Models.Database;

public class Item
{
    public Int128 Quantity { get; set; }

    public Offering Offering { get; set; }
}