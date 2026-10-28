using OopDeepDive.Models;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 02: OBJECT-ORIENTED PROGRAMMING (OOP) DEEP DIVE");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. INHERITANCE & POLYMORPHISM IN ACTION
// ----------------------------------------------------------------------------
PrintHeader("1. Polymorphism & Inheritance");

// Heterogeneous list of BankAccount references
List<BankAccount> accounts =
[
    new SavingsAccount("Alice Johnson", 1000.00m, interestRate: 4.5m)
    {
        AccountNumber = "SAV-9001"
    },
    new CheckingAccount("Bob Smith", 250.00m, overdraftLimit: 500.00m)
    {
        AccountNumber = "CHK-4002"
    }
];

foreach (var account in accounts)
{
    Console.WriteLine($"\nProcessing statement for: {account.AccountHolderName}");
    account.PrintAccountStatement(); // Calls polymorphic overridden method!
    
    // Deposit
    account.Deposit(200.00m);

    // Polymorphic type-testing & pattern matching
    if (account is SavingsAccount savings)
    {
        savings.ApplyMonthlyInterest();
    }
    else if (account is CheckingAccount checking)
    {
        // Try a withdrawal that enters overdraft
        checking.Withdraw(800.00m);
    }
}

// ----------------------------------------------------------------------------
// 2. INTERFACE DISPATCH & DEFAULT INTERFACE METHODS (DIM)
// ----------------------------------------------------------------------------
PrintHeader("2. Interface Dispatch & Default Methods");

foreach (IPayable payable in accounts)
{
    // Calling default interface method defined on IPayable
    payable.PrintReceipt();
}

// ----------------------------------------------------------------------------
// 3. EXPLICIT INTERFACE IMPLEMENTATION
// ----------------------------------------------------------------------------
PrintHeader("3. Explicit Interface Implementation (Disambiguation)");

CheckingAccount checkingAcc = new("Carol Danvers", 500m, 300m)
{
    AccountNumber = "CHK-7701"
};

// checkingAcc.SendAlert(...) -> does NOT compile directly because SendAlert is explicitly implemented!
INotificationTarget notificationRef = checkingAcc;
notificationRef.SendAlert("Your direct deposit has arrived.");

IAuditLogTarget auditRef = checkingAcc;
auditRef.SendAlert("Unauthorized PIN attempt detected.");

// ----------------------------------------------------------------------------
// 4. CLASS VS STRUCT VS RECORD: EQUALITY & MUTATION
// ----------------------------------------------------------------------------
PrintHeader("4. Class vs Record vs Struct (Equality & Immutability)");

// Standard Classes (Reference Equality)
var c1 = new StandardCustomerClass(1, "David");
var c2 = new StandardCustomerClass(1, "David");
Console.WriteLine($"[Class Equality] c1 == c2: {c1 == c2} (False! Separate memory locations)");

// Positional Records (Value-based Equality)
var r1 = new Customer(1, "David", "david@example.com", "Gold");
var r2 = new Customer(1, "David", "david@example.com", "Gold");
Console.WriteLine($"[Record Equality] r1 == r2: {r1 == r2} (True! Automatically compares values)");
Console.WriteLine($"[Record ToString]: {r1}");

// Non-destructive mutation using 'with' expression
Customer upgradedCustomer = r1 with { Tier = "Platinum" };
Console.WriteLine($"[Record 'with' Mutation]: {upgradedCustomer}");
Console.WriteLine($"[Original Record Unchanged]: {r1}");

// Readonly Record Struct
Money price1 = new(49.99m, "USD");
Money price2 = new(49.99m, "USD");
Console.WriteLine($"[Record Struct Equality] price1 == price2: {price1 == price2} (True!)");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 02 OOP Deep Dive completed successfully!\n");
Console.ResetColor();

static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}
