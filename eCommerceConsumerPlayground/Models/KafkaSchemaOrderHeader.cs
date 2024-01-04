using ECommerceConsumerPlayground.Models;

namespace paymentWorker.Models;

public class KafkaSchemaOrderHeader
{
    public String Source { get; set; }
    public long Timestamp { get; set; }
    public string Operation { get; set; }
}