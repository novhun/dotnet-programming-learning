namespace OopDeepDive.Models;

/// <summary>
/// Interface representing an entity capable of processing payments.
/// Demonstrates Default Interface Methods (DIM).
/// </summary>
public interface IPayable
{
    decimal GetPaymentAmount();

    // Default Interface Method (introduced in C# 8)
    void PrintReceipt()
    {
        Console.WriteLine($"[Receipt] Payment processed for amount: {GetPaymentAmount():C}");
    }
}

/// <summary>
/// Interface for receiving notifications. Used to demonstrate Explicit Interface Implementation.
/// </summary>
public interface INotificationTarget
{
    void SendAlert(string message);
}

public interface IAuditLogTarget
{
    void SendAlert(string message);
}

/// <summary>
/// Abstract base class illustrating encapsulation, inheritance, and polymorphic methods.
/// </summary>
public abstract class BankAccount : IPayable
{
    // Encapsulation: Private backing field with controlled property access
    private decimal _balance;

    // Required property (C# 11) - must be initialized when object is instantiated
    public required string AccountNumber { get; init; }

    // Init-only property: can only be assigned during object construction
    public string AccountHolderName { get; init; }

    public decimal Balance => _balance;

    protected BankAccount(string accountHolderName, decimal initialDeposit)
    {
        if (string.IsNullOrWhiteSpace(accountHolderName))
            throw new ArgumentException("Account holder name cannot be empty.", nameof(accountHolderName));

        if (initialDeposit < 0)
            throw new ArgumentException("Initial deposit cannot be negative.", nameof(initialDeposit));

        AccountHolderName = accountHolderName;
        _balance = initialDeposit;
    }

    // Public method mutating private state safely
    public virtual void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");

        _balance += amount;
        Console.WriteLine($"[{AccountNumber}] Deposited {amount:C}. New Balance: {_balance:C}");
    }

    // Abstract method: subclasses MUST provide concrete implementation
    public abstract bool Withdraw(decimal amount);

    // Virtual method: subclasses CAN override, but have a default implementation
    public virtual void PrintAccountStatement()
    {
        Console.WriteLine($"Account: {AccountNumber} | Holder: {AccountHolderName} | Balance: {_balance:C}");
    }

    // IPayable implementation
    public decimal GetPaymentAmount() => _balance;

    // Protected helper for child classes to modify balance safely
    protected void AdjustBalanceDirectly(decimal delta)
    {
        _balance += delta;
    }
}

/// <summary>
/// Concrete derived class for Savings Account with interest calculation.
/// </summary>
public class SavingsAccount : BankAccount
{
    public decimal InterestRate { get; }

    public SavingsAccount(string accountHolderName, decimal initialDeposit, decimal interestRate)
        : base(accountHolderName, initialDeposit)
    {
        InterestRate = interestRate;
    }

    public override bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("[Error] Withdrawal amount must be greater than zero.");
            return false;
        }

        if (Balance - amount < 50m) // Must keep minimum $50 balance
        {
            Console.WriteLine($"[Declined] Savings account requires minimum $50.00 balance. Current: {Balance:C}, Requested: {amount:C}");
            return false;
        }

        AdjustBalanceDirectly(-amount);
        Console.WriteLine($"[{AccountNumber}] Savings withdrawal of {amount:C} approved. Balance: {Balance:C}");
        return true;
    }

    public void ApplyMonthlyInterest()
    {
        decimal interestEarned = Balance * (InterestRate / 100m);
        AdjustBalanceDirectly(interestEarned);
        Console.WriteLine($"[{AccountNumber}] Interest applied ({InterestRate}%): +{interestEarned:C}. Balance: {Balance:C}");
    }

    public override void PrintAccountStatement()
    {
        base.PrintAccountStatement();
        Console.WriteLine($"  -> Account Type: Savings | Interest Rate: {InterestRate}%");
    }
}

/// <summary>
/// Concrete derived class for Checking Account with overdraft protection.
/// Demonstrates sealed method and Explicit Interface Implementation.
/// </summary>
public sealed class CheckingAccount : BankAccount, INotificationTarget, IAuditLogTarget
{
    public decimal OverdraftLimit { get; }

    public CheckingAccount(string accountHolderName, decimal initialDeposit, decimal overdraftLimit)
        : base(accountHolderName, initialDeposit)
    {
        OverdraftLimit = overdraftLimit;
    }

    public override bool Withdraw(decimal amount)
    {
        if (amount <= 0) return false;

        if (Balance - amount < -OverdraftLimit)
        {
            Console.WriteLine($"[Overdraft Limit Reached] Cannot withdraw {amount:C}. Limit: {OverdraftLimit:C}, Balance: {Balance:C}");
            return false;
        }

        AdjustBalanceDirectly(-amount);
        Console.WriteLine($"[{AccountNumber}] Checking withdrawal of {amount:C} approved. Balance: {Balance:C}");
        return true;
    }

    // Explicit Interface Implementation: Resolves naming conflict between INotificationTarget and IAuditLogTarget
    void INotificationTarget.SendAlert(string message)
    {
        Console.WriteLine($"[Customer Notification Push] Message to {AccountHolderName}: {message}");
    }

    void IAuditLogTarget.SendAlert(string message)
    {
        Console.WriteLine($"[Security Audit Log] Event on account {AccountNumber}: {message}");
    }
}
