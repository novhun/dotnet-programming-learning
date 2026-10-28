namespace CleanArchitectureDemo.Application.Contracts;

using CleanArchitectureDemo.Domain.Entities;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveAsync(Wallet wallet, CancellationToken cancellationToken = default);
}

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<CleanArchitectureDemo.Domain.Common.IDomainEvent> events, CancellationToken cancellationToken = default);
}
