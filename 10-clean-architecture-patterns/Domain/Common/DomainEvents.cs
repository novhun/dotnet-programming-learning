namespace CleanArchitectureDemo.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public record WalletDebitedEvent(Guid WalletId, decimal Amount, DateTime OccurredOn) : IDomainEvent;
public record WalletCreditedEvent(Guid WalletId, decimal Amount, DateTime OccurredOn) : IDomainEvent;
