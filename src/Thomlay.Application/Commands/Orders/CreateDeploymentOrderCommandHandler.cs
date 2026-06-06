using Thomlay.Application.Abstractions.Messaging;
using Thomlay.Application.Abstractions.Repositories;
using Thomlay.Domain.Entities;

namespace Thomlay.Application.Commands.Orders;

public class CreateDeploymentOrderCommandHandler 
    : ICommandHandler<CreateDeploymentOrderCommand, Guid>
{
    private readonly IDeploymentOrderRepository _repository;

    // Inject Interface, không inject class cụ thể (Dependency Injection)
    public CreateDeploymentOrderCommandHandler(IDeploymentOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> HandleAsync(
        CreateDeploymentOrderCommand command, 
        CancellationToken cancellationToken = default)
    {
        // 1. Khởi tạo Entity thông qua Domain Logic (đảm bảo tính toàn vẹn dữ liệu)
        // Mặc định trạng thái sẽ là OriginCrafting (Đang chế tác)
        var newOrder = DeploymentOrder.InitiateDeployment(
            command.ArmoryItemId,
            command.CustomerEmail,
            command.BaseAddress,
            command.StripeSessionId
        );

        // 2. Lưu vào Database thông qua Repository
        await _repository.AddAsync(newOrder, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 3. Có thể bắn thêm Event (Domain Event) ở đây nếu sau này 
        // muốn gửi Email tự động "Trang bị của bạn đang được chế tác"

        return newOrder.Id;
    }
}