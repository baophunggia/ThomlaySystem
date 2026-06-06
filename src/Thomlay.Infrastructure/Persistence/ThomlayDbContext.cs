using Microsoft.EntityFrameworkCore;
using Thomlay.Domain.Entities;

namespace Thomlay.Infrastructure.Persistence;

public class ThomlayDbContext : DbContext
{
    public ThomlayDbContext(DbContextOptions<ThomlayDbContext> options) : base(options) { }

    // Khai báo các bảng
    public DbSet<ArmoryItem> ArmoryItems { get; set; }
    public DbSet<DeploymentOrder> DeploymentOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng DeploymentOrder
        modelBuilder.Entity<DeploymentOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(150);
            entity.Property(e => e.BaseAddress).IsRequired();
            // Lưu Enum dưới dạng chuỗi (String) để dễ đọc trong Supabase thay vì số (Integer)
            entity.Property(e => e.Status).HasConversion<string>(); 
        });

        // Cấu hình bảng ArmoryItem
        modelBuilder.Entity<ArmoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SkuCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PriceInUsd).HasColumnType("decimal(18,2)");
        });
    }
}