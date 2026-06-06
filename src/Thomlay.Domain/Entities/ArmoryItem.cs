namespace Thomlay.Domain.Entities;

public class ArmoryItem
{
    public Guid Id { get; private set; }
    public string SkuCode { get; private set; }
    public string Name { get; private set; }
    public string AuraEffect { get; private set; }
    public decimal PriceInUsd { get; private set; } // Thanh toán thẳng bằng USD
    public bool IsActive { get; private set; }

    // Constructor private để ép buộc việc tạo Entity qua Method chuẩn (Domain-Driven Design)
    private ArmoryItem() { }

    public static ArmoryItem Create(string skuCode, string name, string auraEffect, decimal price)
    {
        return new ArmoryItem
        {
            Id = Guid.NewGuid(),
            SkuCode = skuCode,
            Name = name,
            AuraEffect = auraEffect,
            PriceInUsd = price,
            IsActive = true
        };
    }
}