using ECommerceConsumerPlayground.Models;

namespace paymentWorker.Models;

public class KafkaSchemaShoppingBasket
{
    public String Source { get; set; }
    public long Timestamp { get; set; }
    public string Type { get; set; }
    public ShoppingBasket ShoppingBasket { get; set; }
}