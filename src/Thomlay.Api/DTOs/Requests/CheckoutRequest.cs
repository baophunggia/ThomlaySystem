namespace Thomlay.Api.DTOs.Requests
{
    // Model nhận dữ liệu từ Frontend truyền lên
    public class CheckoutRequest
    {
        public Guid ArmoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal PriceInUsd { get; set; }
    }
}