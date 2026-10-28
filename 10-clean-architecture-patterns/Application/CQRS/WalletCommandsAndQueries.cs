namespace CleanArchitectureDemo.Application.CQRS;

using CleanArchitectureDemo.Application.Contracts;
using CleanArchitectureDemo.Domain.Common;

// ----------------------------------------------------------------------------
// COMMAND: TransferFundsCommand & Handler (Write / Mutation)
// ----------------------------------------------------------------------------
public record TransferFundsCommand(
    Guid SourceWalletId,
    Guid DestinationWalletId,
    decimal Amount
);

public record TransferResult(
    Guid SourceWalletId,
    Guid DestinationWalletId,
    decimal Amount,
    decimal SourceNewBalance,
    decimal DestinationNewBalance
);

public class TransferFundsCommandHandler
{
    private readonly IWalletRepository _walletRepo;
    private readonly IDomainEventDispatcher _dispatcher;

    public TransferFundsCommandHandler(IWalletRepository walletRepo, IDomainEventDispatcher dispatcher)
    {
        _walletRepo = walletRepo;
        _dispatcher = dispatcher;
    }

    public async Task<Result<TransferResult>> HandleAsync(TransferFundsCommand command, CancellationToken ct = default)
    {
        if (command.SourceWalletId == command.DestinationWalletId)
        {
            return Result<TransferResult>.Failure(new Error("Transfer.SameWallet", "Cannot transfer funds to the same wallet."));
        }

        var sourceWallet = await _walletRepo.GetByIdAsync(command.SourceWalletId, ct);
        if (sourceWallet is null)
        {
            return Result<TransferResult>.Failure(new Error("Wallet.NotFound", $"Source wallet {command.SourceWalletId} was not found."));
        }

        var destWallet = await _walletRepo.GetByIdAsync(command.DestinationWalletId, ct);
        if (destWallet is null)
        {
            return Result<TransferResult>.Failure(new Error("Wallet.NotFound", $"Destination wallet {command.DestinationWalletId} was not found."));
        }

        // Domain operation: Debit source
        var debitResult = sourceWallet.Withdraw(command.Amount);
        if (debitResult.IsFailure)
        {
            return Result<TransferResult>.Failure(debitResult.Error);
        }

        // Domain operation: Credit destination
        var creditResult = destWallet.Deposit(command.Amount);
        if (creditResult.IsFailure)
        {
            // Rollback withdrawal (in production with DB this is wrapped in a transaction)
            sourceWallet.Deposit(command.Amount);
            return Result<TransferResult>.Failure(creditResult.Error);
        }

        // Persist changes
        await _walletRepo.SaveAsync(sourceWallet, ct);
        await _walletRepo.SaveAsync(destWallet, ct);

        // Dispatch domain events
        await _dispatcher.DispatchAsync(sourceWallet.DomainEvents.Concat(destWallet.DomainEvents), ct);
        sourceWallet.ClearDomainEvents();
        destWallet.ClearDomainEvents();

        return Result<TransferResult>.Success(new TransferResult(
            sourceWallet.Id,
            destWallet.Id,
            command.Amount,
            sourceWallet.Balance,
            destWallet.Balance
        ));
    }
}

// ----------------------------------------------------------------------------
// QUERY: GetWalletBalanceQuery & Handler (Read / Query)
// ----------------------------------------------------------------------------
public record GetWalletBalanceQuery(Guid WalletId);
public record WalletBalanceResponse(Guid WalletId, string OwnerName, decimal Balance, string Currency);

public class GetWalletBalanceQueryHandler
{
    private readonly IWalletRepository _walletRepo;

    public GetWalletBalanceQueryHandler(IWalletRepository walletRepo)
    {
        _walletRepo = walletRepo;
    }

    public async Task<Result<WalletBalanceResponse>> HandleAsync(GetWalletBalanceQuery query, CancellationToken ct = default)
    {
        var wallet = await _walletRepo.GetByIdAsync(query.WalletId, ct);
        if (wallet is null)
        {
            return Result<WalletBalanceResponse>.Failure(new Error("Wallet.NotFound", $"Wallet '{query.WalletId}' not found."));
        }

        return Result<WalletBalanceResponse>.Success(new WalletBalanceResponse(
            wallet.Id,
            wallet.OwnerName,
            wallet.Balance,
            wallet.Currency
        ));
    }
}
