using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Thomlay.Api.DTOs.Requests;

namespace Thomlay.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public CheckoutController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("create-session")]
    public ActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var domain = _configuration["Domain"]; // Tạm thời để localhost của Frontend (Vue/React)

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        // Quy định tiền tệ là USD cho thị trường Mỹ
                        Currency = "usd",
                        UnitAmount = (long)(request.PriceInUsd * 100), // Stripe tính bằng cent (nhân 100)
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.ItemName,
                            Description = "Vật phẩm đang được chế tác tại Cội nguồn Thomlay"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            // Yêu cầu khách nhập địa chỉ giao hàng (Căn cứ thực tế) ngay trên form Stripe
            BillingAddressCollection = "required",
            ShippingAddressCollection = new SessionShippingAddressCollectionOptions
            {
                AllowedCountries = new List<string> { "US" } // Chỉ cho phép ship nội địa Mỹ
            },
            SuccessUrl = domain + "/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = domain + "/armory", // Trở về kho vũ khí nếu hủy

            // Gói siêu dữ liệu (Metadata) cực kỳ quan trọng để Webhook biết khách mua món gì
            Metadata = new Dictionary<string, string>
            {
                { "ArmoryItemId", request.ArmoryItemId.ToString() }
            }
        };

        var service = new SessionService();
        Session session = service.Create(options);

        // Trả về cái Link (URL) của Stripe để Frontend tự động redirect người dùng sang đó
        return Ok(new { url = session.Url });
    }
}

