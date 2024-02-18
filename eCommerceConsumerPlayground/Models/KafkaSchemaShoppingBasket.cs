using ECommerceConsumerPlayground.Models;

namespace paymentWorker.Models;

public class KafkaSchemaShoppingBasket
{
    public String source { get; set; }
    public long timestamp { get; set; }
    public string operation { get; set; }
    public ShoppingBasket ShoppingBasket { get; set; }
}