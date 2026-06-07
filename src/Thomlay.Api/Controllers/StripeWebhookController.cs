using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Thomlay.Application.Commands.Orders;

namespace Thomlay.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
public class StripeWebhookController : ControllerBase
{
    private readonly CreateDeploymentOrderCommandHandler _handler;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        CreateDeploymentOrderCommandHandler handler,
        IConfiguration configuration,
        ILogger<StripeWebhookController> logger)
    {
        _handler = handler;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var endpointSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                endpointSecret,
                throwOnApiVersionMismatch: false
            );

            _logger.LogInformation("Received Stripe event: {EventType}", stripeEvent.Type);

            // Chỉ xử lý event này trong giai đoạn hiện tại
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                await HandleCheckoutSessionCompleted(stripeEvent);
            }
            else
            {
                _logger.LogInformation("Ignored event: {EventType}", stripeEvent.Type);
            }

            return Ok(); // Phải trả về 200 nhanh
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe signature verification failed");
            return BadRequest("Invalid signature");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return StatusCode(500);
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Session;

        if (session?.PaymentStatus != "paid")
        {
            _logger.LogWarning("Session not paid: {SessionId}", session?.Id);
            return;
        }

        var command = new CreateDeploymentOrderCommand
        {
            StripeSessionId = session.Id,
            CustomerEmail = session.CustomerDetails?.Email ?? string.Empty,
            ArmoryItemId = Guid.Parse(session.Metadata["ArmoryItemId"]
                ?? throw new InvalidOperationException("Missing ArmoryItemId in metadata")),
            BaseAddress = session.CustomerDetails?.Address?.Line1
                         ?? "Địa chỉ chưa xác định"
        };

        await _handler.HandleAsync(command);
        _logger.LogInformation("✅ Order created successfully for session {SessionId}", session.Id);
    }
}