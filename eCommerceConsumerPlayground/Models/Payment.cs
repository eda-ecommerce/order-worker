using eCommerceConsumerPlayground.Models;
namespace ECommerceConsumerPlayground.Models;

public class Payment
{
    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public DateOnly CreatedDate { get; set; }
    public PaymentStatus Status { get; set; }

    private Order Order { get; set; }

}
