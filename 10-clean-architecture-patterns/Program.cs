using CleanArchitectureDemo.Application.CQRS;
using CleanArchitectureDemo.Domain.Entities;
using CleanArchitectureDemo.Infrastructure.Persistence;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 10: CLEAN ARCHITECTURE & ENTERPRISE PATTERNS");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// Initialize Infrastructure dependencies
var walletRepository = new InMemoryWalletRepository();
var eventDispatcher = new ConsoleDomainEventDispatcher();

// Initialize CQRS Handlers
var transferHandler = new TransferFundsCommandHandler(walletRepository, eventDispatcher);
var queryHandler = new GetWalletBalanceQueryHandler(walletRepository);

// ----------------------------------------------------------------------------
// 1. DOMAIN AGGREGATES SETUP
// ----------------------------------------------------------------------------
PrintHeader("1. Initializing Domain Aggregate Roots");

var aliceWalletId = Guid.NewGuid();
var bobWalletId = Guid.NewGuid();

var aliceWallet = new Wallet(aliceWalletId, "Alice", "USD", initialBalance: 500.00m);
var bobWallet = new Wallet(bobWalletId, "Bob", "USD", initialBalance: 100.00m);

await walletRepository.SaveAsync(aliceWallet);
await walletRepository.SaveAsync(bobWallet);

Console.WriteLine($"Initialized Alice's wallet: {aliceWallet.Balance:C}");
Console.WriteLine($"Initialized Bob's wallet:   {bobWallet.Balance:C}");

// ----------------------------------------------------------------------------
// 2. EXECUTING CQRS COMMAND (WRITE PIPELINE)
// ----------------------------------------------------------------------------
PrintHeader("2. Executing CQRS Command: TransferFundsCommand ($150)");

var successfulCommand = new TransferFundsCommand(aliceWalletId, bobWalletId, 150.00m);
var transferResult = await transferHandler.HandleAsync(successfulCommand);

transferResult.Match(
    onSuccess: res =>
    {
        Console.WriteLine($"  [SUCCESS] Transferred {res.Amount:C} from Alice to Bob!");
        Console.WriteLine($"  -> Alice new balance: {res.SourceNewBalance:C}");
        Console.WriteLine($"  -> Bob new balance:   {res.DestinationNewBalance:C}");
        return true;
    },
    onFailure: err =>
    {
        Console.WriteLine($"  [FAILED] Code: {err.Code}, Message: {err.Message}");
        return false;
    }
);

// ----------------------------------------------------------------------------
// 3. EXECUTING CQRS QUERY (READ PIPELINE)
// ----------------------------------------------------------------------------
PrintHeader("3. Executing CQRS Query: GetWalletBalanceQuery");

var aliceBalanceQuery = await queryHandler.HandleAsync(new GetWalletBalanceQuery(aliceWalletId));
var bobBalanceQuery = await queryHandler.HandleAsync(new GetWalletBalanceQuery(bobWalletId));

Console.WriteLine($"  [Query Result] {aliceBalanceQuery.Value?.OwnerName}: {aliceBalanceQuery.Value?.Balance:C} {aliceBalanceQuery.Value?.Currency}");
Console.WriteLine($"  [Query Result] {bobBalanceQuery.Value?.OwnerName}:   {bobBalanceQuery.Value?.Balance:C} {bobBalanceQuery.Value?.Currency}");

// ----------------------------------------------------------------------------
// 4. RAILWAY-ORIENTED PROGRAMMING: FUNCTIONAL ERROR HANDLING
// ----------------------------------------------------------------------------
PrintHeader("4. Error Handling via Result<T> Pattern (No Exceptions!)");

// Attempt to transfer more than Alice has left ($350 available, requesting $1,000)
var overdrawnCommand = new TransferFundsCommand(aliceWalletId, bobWalletId, 1000.00m);
var failedResult = await transferHandler.HandleAsync(overdrawnCommand);

failedResult.Match(
    onSuccess: _ =>
    {
        Console.WriteLine("Unexpected success!");
        return true;
    },
    onFailure: err =>
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [Handled Business Error via Result Pattern]");
        Console.WriteLine($"  Error Code:    {err.Code}");
        Console.WriteLine($"  Error Message: {err.Message}");
        Console.ResetColor();
        return false;
    }
);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 10 Clean Architecture completed successfully!\n");
Console.ResetColor();

static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}
