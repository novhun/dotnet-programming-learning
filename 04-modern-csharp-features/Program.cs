using ModernCSharp.Events;
using ModernCSharp.Extensions;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 04: MODERN C# FEATURES & FUNCTIONAL CONCEPTS");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. DELEGATES, FUNC, ACTION & CLOSURES
// ----------------------------------------------------------------------------
PrintHeader("1. Delegates, Func<T>, Action<T> and Closures");

// Action: returns void
Action<string> logger = msg => Console.WriteLine($"[LOG - {DateTime.UtcNow:HH:mm:ss}] {msg}");
logger("Application booting...");

// Func: returns a value
Func<decimal, decimal, decimal> taxCalculator = (amount, rate) => amount * (rate / 100m);
Console.WriteLine($"[Func Tax Calculation] $250 at 8% tax: {taxCalculator(250m, 8m):C}");

// Closures: Lambda capturing outer variable
int multiplier = 3;
Func<int, int> multiply = x => x * multiplier;
Console.WriteLine($"[Closure with multiplier = 3]: 10 * 3 = {multiply(10)}");
multiplier = 10;
Console.WriteLine($"[Closure after mutating outer variable]: 10 * 10 = {multiply(10)}");

// ----------------------------------------------------------------------------
// 2. EVENTS (PUBLISHER - SUBSCRIBER PATTERN)
// ----------------------------------------------------------------------------
PrintHeader("2. Events and Decoupled Notifications");

var orderService = new OrderService();

// Subscriber 1: Email Notification
orderService.OrderCompleted += (sender, e) =>
{
    Console.WriteLine($"  -> [Email Service] Sending receipt for order {e.OrderNumber} ({e.Amount:C})");
};

// Subscriber 2: Analytics Tracking
orderService.OrderCompleted += (sender, e) =>
{
    Console.WriteLine($"  -> [Analytics Service] Tracking conversion: {e.Amount:C} at {e.ProcessedAt:T}");
};

orderService.CompleteOrder("ORD-2026-X99", 349.99m);

// ----------------------------------------------------------------------------
// 3. ADVANCED PATTERN MATCHING (PROPERTY, RELATIONAL, LIST PATTERNS)
// ----------------------------------------------------------------------------
PrintHeader("3. Advanced Pattern Matching");

// 3a. Property & Relational Patterns
UserAccount vipUser = new("Sarah", Role: "Admin", Level: 5, IsSuspended: false);
UserAccount suspendedUser = new("Mallory", Role: "Admin", Level: 10, IsSuspended: true);

Console.WriteLine($"[Sarah access]: {EvaluateAccess(vipUser)}");
Console.WriteLine($"[Mallory access]: {EvaluateAccess(suspendedUser)}");

// 3b. List Patterns (C# 11+)
int[] numbers1 = [1, 2, 3, 4, 5];
int[] numbers2 = [1, 99];
int[] emptyList = [];

Console.WriteLine($"[1, 2, 3, 4, 5] matches: {MatchList(numbers1)}");
Console.WriteLine($"[1, 99] matches: {MatchList(numbers2)}");
Console.WriteLine($"[] matches: {MatchList(emptyList)}");

// ----------------------------------------------------------------------------
// 4. EXTENSION METHODS IN ACTION
// ----------------------------------------------------------------------------
PrintHeader("4. Fluent Extension Methods");

string articleTitle = "Mastering Modern C# 13 and .NET 10 Development!";
Console.WriteLine($"Original Title: \"{articleTitle}\"");
Console.WriteLine($"To Slug:        \"{articleTitle.ToSlug()}\"");
Console.WriteLine($"Truncate (25):  \"{articleTitle.Truncate(25)}\"");

int temperature = 24;
Console.WriteLine($"Is 24C between 18C and 26C? {temperature.IsBetween(18, 26)}");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 04 Modern C# Features completed successfully!\n");
Console.ResetColor();

// ============================================================================
// HELPER MODELS & LOCAL FUNCTIONS
// ============================================================================
static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}

static string EvaluateAccess(UserAccount user) => user switch
{
    { IsSuspended: true } => "BLOCKED: Account is suspended.",
    { Role: "Admin", Level: >= 5 } => "GRANTED: Super Administrator privileges.",
    { Role: "Admin", Level: < 5 } => "GRANTED: Basic Administrator privileges.",
    { Role: "User" } => "GRANTED: Standard User privileges.",
    _ => "DENIED: Unknown user role."
};

static string MatchList(int[] values) => values switch
{
    [] => "Empty collection",
    [var single] => $"Single item: {single}",
    [1, 2, .. var rest] => $"Starts with 1, 2 followed by {rest.Length} elements",
    [var first, .., var last] => $"Starts with {first} and ends with {last}",
    _ => "Unmatched pattern"
};

public record UserAccount(string Name, string Role, int Level, bool IsSuspended);
