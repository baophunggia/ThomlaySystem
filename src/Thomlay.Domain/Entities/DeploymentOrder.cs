using Thomlay.Domain.Enums;

namespace Thomlay.Domain.Entities;

public class DeploymentOrder
{
    public Guid Id { get; private set; }
    public Guid ArmoryItemId { get; private set; }
    public string CustomerEmail { get; private set; }
    public string BaseAddress { get; private set; } // Địa chỉ Căn cứ thực tế
    public DeploymentStatus Status { get; private set; }
    public string StripeSessionId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DeploymentOrder() { }

    public static DeploymentOrder InitiateDeployment(Guid itemId, string email, string address, string stripeSessionId)
    {
        return new DeploymentOrder
        {
            Id = Guid.NewGuid(),
            ArmoryItemId = itemId,
            CustomerEmail = email,
            BaseAddress = address,
            StripeSessionId = stripeSessionId,
            Status = DeploymentStatus.OriginCrafting, // Luôn bắt đầu ở khâu Chế tác
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AdvanceDeploymentStatus(DeploymentStatus newStatus)
    {
        // Business Rule: Chỉ cho phép đi tới, không được quay lui trạng thái
        if (newStatus > Status) 
        {
            Status = newStatus;
        }
    }
}