namespace Thomlay.Domain.Enums;

public enum DeploymentStatus
{
    OriginCrafting = 1,   // Đang chế tác tại Cội nguồn
    OceanTransit = 2,     // Vượt đại dương
    CustomsClearance = 3, // Vượt Tường Lửa (Thông quan)
    BaseDeployment = 4    // Tiếp cận Căn cứ (Đang giao nội địa)
}