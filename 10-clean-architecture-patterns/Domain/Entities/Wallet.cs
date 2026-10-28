namespace CleanArchitectureDemo.Domain.Entities;

using CleanArchitectureDemo.Domain.Common;

/// <summary>
/// Domain Aggregate Root: Encapsulates business invariants and raises domain events.
/// </summary>
public class Wallet
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; }
    public string OwnerName { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Wallet(Guid id, string ownerName, string currency, decimal initialBalance = 0m)
    {
        Id = id;
        OwnerName = ownerName;
        Currency = currency;
        Balance = initialBalance;
    }

    public Result<decimal> Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            return Result<decimal>.Failure(new Error("Wallet.InvalidAmount", "Deposit amount must be greater than zero."));
        }

        Balance += amount;
        _domainEvents.Add(new WalletCreditedEvent(Id, amount, DateTime.UtcNow));
        return Result<decimal>.Success(Balance);
    }

    public Result<decimal> Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            return Result<decimal>.Failure(new Error("Wallet.InvalidAmount", "Withdrawal amount must be greater than zero."));
        }

        if (Balance < amount)
        {
            return Result<decimal>.Failure(new Error("Wallet.InsufficientFunds", $"Insufficient funds. Current balance: {Balance:C}, Requested: {amount:C}."));
        }

        Balance -= amount;
        _domainEvents.Add(new WalletDebitedEvent(Id, amount, DateTime.UtcNow));
        return Result<decimal>.Success(Balance);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
