using Thomlay.Application.Abstractions.Messaging;

namespace Thomlay.Application.Commands.Orders;

// Command này mang theo dữ liệu chiết xuất từ Stripe Webhook
// Trả về Guid (chính là ID của đơn hàng vừa được tạo)
public class CreateDeploymentOrderCommand : ICommand<Guid>
{
    public Guid ArmoryItemId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string BaseAddress { get; set; } = string.Empty;
    public string StripeSessionId { get; set; } = string.Empty;
}