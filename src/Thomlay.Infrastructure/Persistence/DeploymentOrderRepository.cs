using Microsoft.EntityFrameworkCore;
using Thomlay.Application.Abstractions.Repositories;
using Thomlay.Domain.Entities;

namespace Thomlay.Infrastructure.Persistence;

public class DeploymentOrderRepository : IDeploymentOrderRepository
{
    private readonly ThomlayDbContext _dbContext;

    public DeploymentOrderRepository(ThomlayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(DeploymentOrder order, CancellationToken cancellationToken = default)
    {
        await _dbContext.DeploymentOrders.AddAsync(order, cancellationToken);
    }

    public async Task<DeploymentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DeploymentOrders
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}