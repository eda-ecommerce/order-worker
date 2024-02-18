using ECommerceConsumerPlayground.Models;

namespace paymentWorker.Models;

public class KafkaSchemaOrderHeader
{
    public String source { get; set; }
    public long timestamp { get; set; }
    public string operation { get; set; }
}