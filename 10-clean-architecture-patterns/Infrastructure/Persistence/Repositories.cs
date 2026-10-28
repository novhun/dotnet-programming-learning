namespace CleanArchitectureDemo.Infrastructure.Persistence;

using System.Collections.Concurrent;
using CleanArchitectureDemo.Application.Contracts;
using CleanArchitectureDemo.Domain.Common;
using CleanArchitectureDemo.Domain.Entities;

public class InMemoryWalletRepository : IWalletRepository
{
    private readonly ConcurrentDictionary<Guid, Wallet> _wallets = new();

    public Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _wallets.TryGetValue(id, out var wallet);
        return Task.FromResult(wallet);
    }

    public Task SaveAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        _wallets[wallet.Id] = wallet;
        return Task.CompletedTask;
    }
}

public class ConsoleDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var ev in events)
        {
            switch (ev)
            {
                case WalletDebitedEvent debited:
                    Console.WriteLine($"    [Event Dispatched] Wallet {debited.WalletId} debited -{debited.Amount:C} at {debited.OccurredOn:HH:mm:ss.fff}");
                    break;
                case WalletCreditedEvent credited:
                    Console.WriteLine($"    [Event Dispatched] Wallet {credited.WalletId} credited +{credited.Amount:C} at {credited.OccurredOn:HH:mm:ss.fff}");
                    break;
            }
        }
        return Task.CompletedTask;
    }
}
