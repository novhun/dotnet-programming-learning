// ============================================================================
// PART 01: C# FUNDAMENTALS - FROM ZERO TO HERO
// ============================================================================

using System.Globalization;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 01: C# LANGUAGE FUNDAMENTALS & ESSENTIAL CONCEPTS");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. DATA TYPES & VARIABLES (Value Types vs Reference Types)
// ----------------------------------------------------------------------------
PrintSectionHeader("1. Data Types: Value Types vs Reference Types");

// Value types live on the Stack (usually) and store data directly
int integerNumber = 42;
double floatingPoint = 3.14159;
decimal financialDecimal = 199.99m; // Precision for financial calculations
bool isLearningDotNet = true;
char grade = 'A';

Console.WriteLine($"[Value Types] int: {integerNumber}, double: {floatingPoint}, decimal: {financialDecimal:C}, bool: {isLearningDotNet}, char: '{grade}'");

// Value copy demonstration
int originalValue = 100;
int copiedValue = originalValue;
copiedValue = 999;
Console.WriteLine($"[Value Copy Behavior] Original: {originalValue}, Copied & Modified: {copiedValue} (Original remains unchanged)");

// Reference types live on the Heap and variables hold pointers to memory
string greeting = "Hello, .NET Developer!";
int[] numbersArray = [1, 2, 3];
int[] aliasArray = numbersArray;
aliasArray[0] = 777;
Console.WriteLine($"[Reference Types] Greeting: \"{greeting}\", originalArray[0]: {numbersArray[0]} (Changed via alias pointer!)");

// ----------------------------------------------------------------------------
// 2. TYPE CASTING & SAFE CONVERSIONS
// ----------------------------------------------------------------------------
PrintSectionHeader("2. Type Casting & Parsing");

// Implicit conversion (safe, no data loss)
int smallInt = 12345;
long bigLong = smallInt;
Console.WriteLine($"[Implicit Casting] int {smallInt} -> long {bigLong}");

// Explicit casting (potential truncation or overflow)
double preciseRating = 4.89;
int truncatedRating = (int)preciseRating;
Console.WriteLine($"[Explicit Casting] double {preciseRating} -> int {truncatedRating}");

// Safe parsing with TryParse pattern
string inputStr = "2026";
if (int.TryParse(inputStr, out int parsedYear))
{
    Console.WriteLine($"[Safe Parsing] Successfully parsed year: {parsedYear}");
}
else
{
    Console.WriteLine("[Safe Parsing] Failed to parse string to integer.");
}

// ----------------------------------------------------------------------------
// 3. NULLABLE TYPES & MODERN NULL OPERATORS
// ----------------------------------------------------------------------------
PrintSectionHeader("3. Nullable Types & Null Safety (? , ?? , ??= , ?.)");

int? optionalScore = null; // Nullable value type (Nullable<int>)
Console.WriteLine($"[Nullable] Score is null? {!optionalScore.HasValue}");

// Null-coalescing operator (??) - fallback default value
int finalScore = optionalScore ?? 0;
Console.WriteLine($"[?? Fallback] Score with fallback: {finalScore}");

// Null-coalescing assignment (??=)
string? username = null;
username ??= "GuestUser";
Console.WriteLine($"[??= Assign If Null] Username assigned: {username}");

// Null-conditional operator (?.)
string? text = null;
int? textLength = text?.Length;
Console.WriteLine($"[?. Safe Navigation] Safe length access on null: {textLength?.ToString() ?? "null"}");

// ----------------------------------------------------------------------------
// 4. CONTROL FLOW & MODERN PATTERN MATCHING SWITCH
// ----------------------------------------------------------------------------
PrintSectionHeader("4. Control Flow & Pattern Matching");

int testScore = 88;
string letterGrade = testScore switch
{
    >= 90 => "A (Outstanding)",
    >= 80 and < 90 => "B (Very Good)",
    >= 70 and < 80 => "C (Good)",
    >= 60 and < 70 => "D (Pass)",
    _ => "F (Retake Required)"
};
Console.WriteLine($"[Switch Expression] Score {testScore} => Grade: {letterGrade}");

// Tuple pattern matching
(string userRole, bool isActive) userSession = ("Admin", true);
string permission = userSession switch
{
    ("Admin", true) => "Full Administrator Access Granted",
    ("Admin", false) => "Admin Account Inactive - Contact Security",
    ("Member", true) => "Standard Access Granted",
    _ => "Access Denied"
};
Console.WriteLine($"[Tuple Pattern] {userSession} => {permission}");

// ----------------------------------------------------------------------------
// 5. LOOPS & ITERATION
// ----------------------------------------------------------------------------
PrintSectionHeader("5. Loops & Iterations (for, foreach, while, do-while)");

// Foreach with collection expression
string[] fruits = ["Apple", "Blueberry", "Cherry", "Dragonfruit"];
Console.Write("[Foreach Loop]: ");
foreach (var fruit in fruits)
{
    Console.Write($"{fruit} | ");
}
Console.WriteLine();

// ----------------------------------------------------------------------------
// 6. METHODS & PARAMETER MODIFIERS (in, out, ref, params)
// ----------------------------------------------------------------------------
PrintSectionHeader("6. Methods & Parameter Modifiers (out, ref, params)");

// out parameter
CalculateCircle(radius: 5.0, out double area, out double circumference);
Console.WriteLine($"[out parameters] Radius: 5.0 => Area: {area:F2}, Circumference: {circumference:F2}");

// ref parameter (mutates original variable)
int balance = 500;
ApplyBonus(ref balance, 150);
Console.WriteLine($"[ref parameter] New balance after bonus: {balance}");

// params parameter (variable argument list)
int totalSum = SumAll(10, 20, 30, 40, 50);
Console.WriteLine($"[params parameter] SumAll(10, 20, 30, 40, 50) = {totalSum}");

// ----------------------------------------------------------------------------
// 7. REAL-WORLD INTERACTIVE EXERCISES
// ----------------------------------------------------------------------------
PrintSectionHeader("7. Interactive Real-World Exercises");
Console.WriteLine("Select an interactive exercise to run:");
Console.WriteLine("  [1] Product Invoice & Tiered Discount Calculator");
Console.WriteLine("  [2] Full Multi-Operation Console Calculator (+, -, *, /, %, ^)");
Console.WriteLine("  [3] Run Both Sequentially");
Console.WriteLine("  [0] Skip Interactive Exercises");
Console.Write("\nEnter choice (1, 2, 3, or 0 - default: 2): ");

string? exerciseChoice = Console.ReadLine()?.Trim();
if (string.IsNullOrWhiteSpace(exerciseChoice)) exerciseChoice = "2";

switch (exerciseChoice)
{
    case "1":
        RunInteractiveInvoiceCalculator();
        break;
    case "2":
        RunFullConsoleCalculator();
        break;
    case "3":
        RunInteractiveInvoiceCalculator();
        RunFullConsoleCalculator();
        break;
    case "0":
        Console.WriteLine("Skipped interactive exercises.");
        break;
    default:
        RunFullConsoleCalculator();
        break;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 01 Fundamentals completed successfully!\n");
Console.ResetColor();

// ============================================================================
// HELPER METHODS
// ============================================================================
static void PrintSectionHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}

static void CalculateCircle(double radius, out double area, out double circumference)
{
    area = Math.PI * radius * radius;
    circumference = 2 * Math.PI * radius;
}

static void ApplyBonus(ref int accountBalance, int bonusAmount)
{
    accountBalance += bonusAmount;
}

static int SumAll(params int[] numbers)
{
    int sum = 0;
    foreach (var n in numbers)
    {
        sum += n;
    }
    return sum;
}

static void RunInteractiveInvoiceCalculator()
{
    Console.WriteLine("Enter details (or press Enter to test with defaults):\n");

    Console.Write("Enter product name: ");
    string? inputName = Console.ReadLine();
    string productName = string.IsNullOrWhiteSpace(inputName) ? "Mechanical Keyboard" : inputName.Trim();
    if (string.IsNullOrWhiteSpace(inputName)) Console.WriteLine(productName);

    Console.Write("Enter quantity: ");
    string? inputQty = Console.ReadLine();
    int qty = int.TryParse(inputQty, out int parsedQty) && parsedQty > 0 ? parsedQty : 60;
    if (string.IsNullOrWhiteSpace(inputQty)) Console.WriteLine(qty);

    Console.Write("Enter unit price: ");
    string? inputPrice = Console.ReadLine();
    double price = double.TryParse(inputPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedPrice) && parsedPrice > 0
        ? parsedPrice
        : 45.00;
    if (string.IsNullOrWhiteSpace(inputPrice)) Console.WriteLine(price.ToString("F2", CultureInfo.InvariantCulture));

    double subtotal = qty * price;
    double discountRate = 0.0;

    if (qty > 100)
    {
        discountRate = 0.20;
    }
    else if (qty > 50)
    {
        discountRate = 0.10;
    }
    else if (qty >= 20)
    {
        discountRate = 0.05;
    }

    double discountAmount = subtotal * discountRate;
    double finalAmount = subtotal - discountAmount;

    Console.WriteLine("\n-----------------------------");
    Console.WriteLine($"Product:         {productName}");
    Console.WriteLine($"Quantity:        {qty}");
    Console.WriteLine($"Unit Price:      ${price:F2}");
    Console.WriteLine($"Subtotal:        ${subtotal:F2}");
    Console.WriteLine($"Discount ({discountRate * 100:0}%): -${discountAmount:F2}");
    Console.WriteLine($"Total Amount:    ${finalAmount:F2}");
    Console.WriteLine("-----------------------------");
}

static void RunFullConsoleCalculator()
{
    bool keepRunning = true;

    try { Console.Title = "C# Full Calculator"; } catch { }
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n========================================");
    Console.WriteLine("          FULL CONSOLE CALCULATOR       ");
    Console.WriteLine("========================================");
    Console.ResetColor();

    while (keepRunning)
    {
        // 1. Get first number
        double num1 = ReadDouble("Enter first number: ");

        // 2. Get valid operator
        char op = ReadOperator();

        // 3. Get second number
        double num2 = ReadDouble("Enter second number: ");

        // 4. Calculate result
        double? result = Calculate(num1, num2, op);

        // 5. Display output
        Console.WriteLine("\n----------------------------------------");
        if (result.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {num1} {op} {num2} = {result.Value:G10}");
            Console.ResetColor();
        }
        Console.WriteLine("----------------------------------------\n");

        // 6. Ask to continue
        Console.Write("Do another calculation? (y/n): ");
        string answer = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";
        if (answer != "y" && answer != "yes")
        {
            keepRunning = false;
        }
        Console.WriteLine();
    }

    Console.WriteLine("Thank you for using the calculator! Goodbye.");
}

// Safely reads and parses a decimal number from the user
static double ReadDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.Trim() ?? "";

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
        {
            return value;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid number! Please enter a valid number (e.g., 10, 3.14, -5).");
        Console.ResetColor();
    }
}

// Safely reads and validates the chosen operator
static char ReadOperator()
{
    while (true)
    {
        Console.Write("Enter operator (+, -, *, /, %, ^): ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (input.Length == 1 && "+-*/%^".Contains(input[0]))
        {
            return input[0];
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid operator! Please choose one of: +, -, *, /, %, ^");
        Console.ResetColor();
    }
}

// Performs the operation and handles math errors
static double? Calculate(double a, double b, char op)
{
    switch (op)
    {
        case '+':
            return a + b;
        case '-':
            return a - b;
        case '*':
            return a * b;
        case '/':
            if (b == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Division by zero is not allowed.");
                Console.ResetColor();
                return null;
            }
            return a / b;
        case '%':
            if (b == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Modulo by zero is not allowed.");
                Console.ResetColor();
                return null;
            }
            return a % b;
        case '^':
            return Math.Pow(a, b);
        default:
            return null;
    }
}
