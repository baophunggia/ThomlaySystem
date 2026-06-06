using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Thomlay.Application.Commands.Orders;

namespace Thomlay.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly CreateDeploymentOrderCommandHandler _handler;
    private readonly IConfiguration _configuration;

    // Inject Handler trực tiếp theo chuẩn Vanilla CQRS
    public StripeWebhookController(CreateDeploymentOrderCommandHandler handler, IConfiguration configuration)
    {
        _handler = handler;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        // 1. Đọc luồng dữ liệu thô từ Stripe gửi tới
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var endpointSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            // 2. Xác thực chữ ký điện tử (Chống giả mạo request, cực kỳ quan trọng cho thị trường Mỹ)
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                endpointSecret
            );

            // 3. Nếu khách hàng thanh toán thành công
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                // Trích xuất dữ liệu do frontend gài vào metadata khi tạo link thanh toán
                var command = new CreateDeploymentOrderCommand
                {
                    StripeSessionId = session.Id,
                    CustomerEmail = session.CustomerDetails?.Email ?? string.Empty,
                    // Giả định frontend sẽ nhét ID vật phẩm vào Metadata
                    ArmoryItemId = Guid.Parse(session.Metadata["ArmoryItemId"]),
                    // Lấy địa chỉ giao hàng do khách nhập thẳng trên form của Stripe
                    BaseAddress = session.CustomerDetails?.Address?.Line1 ?? "Địa chỉ chưa xác định"
                };

                // 4. Kích hoạt luồng tạo đơn hàng trong Database
                await _handler.HandleAsync(command);
            }

            return Ok(); // Phản hồi cho Stripe biết hệ thống Thomlay đã nhận được
        }
        catch (StripeException e)
        {
            // Lỗi xác thực chữ ký (Có người cố tình gọi API trái phép)
            return BadRequest(new { error = e.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Lỗi xử lý nội bộ tại Căn cứ Thomlay" });
        }
    }
}