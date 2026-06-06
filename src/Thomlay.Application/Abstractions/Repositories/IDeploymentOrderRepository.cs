using Thomlay.Domain.Entities;

namespace Thomlay.Application.Abstractions.Repositories;

public interface IDeploymentOrderRepository
{
    // Chỉ định nghĩa CÁI GÌ cần làm, không quan tâm LÀM NHƯ THẾ NÀO
    Task AddAsync(DeploymentOrder order, CancellationToken cancellationToken = default);
    Task<DeploymentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}