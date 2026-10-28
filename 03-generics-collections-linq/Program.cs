using GenericsAndLinq.Generics;
using GenericsAndLinq.Models;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 03: GENERICS, COLLECTIONS & LINQ IN DEPTH");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. GENERICS & CONSTRAINTS DEMONSTRATION
// ----------------------------------------------------------------------------
PrintHeader("1. Generics and Type Constraints");

int numA = 50, numB = 100;
GenericUtils.Swap(ref numA, ref numB);
Console.WriteLine($"[Generic Swap] numA: {numA}, numB: {numB}");

string maxStr = GenericUtils.MaxOf("Apple", "Zebra");
Console.WriteLine($"[Generic MaxOf] Greater string: '{maxStr}'");

var repo = new InMemoryRepository<int, Product>();
repo.AddOrUpdate(new Product(1, "MacBook Pro M4", "Electronics", 1999.00m, 15, 4.9));
repo.AddOrUpdate(new Product(2, "Mechanical Keyboard", "Accessories", 129.99m, 40, 4.7));
repo.AddOrUpdate(new Product(3, "UltraWide Monitor", "Electronics", 699.50m, 8, 4.6));
Console.WriteLine($"[Generic Repository] Stored {repo.Count} products.");

// ----------------------------------------------------------------------------
// 2. ESSENTIAL .NET COLLECTIONS & USE-CASES
// ----------------------------------------------------------------------------
PrintHeader("2. Modern Collections (HashSet, PriorityQueue)");

// HashSet for fast O(1) membership & deduplication
HashSet<string> allowedTags = ["csharp", "dotnet", "cloud", "backend"];
Console.WriteLine($"[HashSet O(1) Check] 'dotnet' allowed? {allowedTags.Contains("dotnet")}");
Console.WriteLine($"[HashSet O(1) Check] 'python' allowed? {allowedTags.Contains("python")}");

// PriorityQueue (.NET 6+) - Elements dequeued by priority (lowest number = highest priority)
PriorityQueue<string, int> taskQueue = new();
taskQueue.Enqueue("Routine Maintenance Task", priority: 3);
taskQueue.Enqueue("CRITICAL: Server Outage", priority: 1);
taskQueue.Enqueue("User Bug Report", priority: 2);

Console.WriteLine("[PriorityQueue Dequeue order]:");
while (taskQueue.TryDequeue(out string? task, out int priority))
{
    Console.WriteLine($"  -> [Priority {priority}] {task}");
}

// ----------------------------------------------------------------------------
// 3. SAMPLE DATASET SETUP FOR LINQ
// ----------------------------------------------------------------------------
List<Product> products =
[
    new Product(1, "MacBook Pro M4", "Computers", 1999.00m, 12, 4.9),
    new Product(2, "Dell XPS 15", "Computers", 1499.00m, 6, 4.5),
    new Product(3, "Sony WH-1000XM5", "Audio", 349.99m, 25, 4.8),
    new Product(4, "AirPods Pro 2", "Audio", 249.00m, 50, 4.7),
    new Product(5, "Ergonomic Chair", "Furniture", 450.00m, 4, 4.3),
    new Product(6, "Standing Desk", "Furniture", 550.00m, 0, 4.4), // Out of stock
    new Product(7, "USB-C Multiport Hub", "Accessories", 49.99m, 100, 4.2)
];

List<CustomerOrder> orders =
[
    new CustomerOrder(101, "Alice", DateTime.UtcNow.AddDays(-2), [
        new OrderItem(1, 1, 1999.00m),
        new OrderItem(4, 2, 249.00m)
    ], true),
    new CustomerOrder(102, "Bob", DateTime.UtcNow.AddDays(-1), [
        new OrderItem(3, 1, 349.99m),
        new OrderItem(7, 3, 49.99m)
    ], true),
    new CustomerOrder(103, "Charlie", DateTime.UtcNow, [
        new OrderItem(1, 1, 1999.00m),
        new OrderItem(5, 1, 450.00m)
    ], false)
];

// ----------------------------------------------------------------------------
// 4. DEFERRED EXECUTION VS IMMEDIATE EXECUTION
// ----------------------------------------------------------------------------
PrintHeader("4. Deferred Execution Demonstration");

var filteredQuery = products.Where(p => p.Price > 500); // Query NOT executed yet!
Console.WriteLine($"Query created (type: {filteredQuery.GetType().Name}). Executing now via iteration:");
foreach (var p in filteredQuery)
{
    Console.WriteLine($"  - {p.Name} ({p.Price:C})");
}

// ----------------------------------------------------------------------------
// 5. ADVANCED LINQ QUERIES
// ----------------------------------------------------------------------------
PrintHeader("5. Advanced LINQ: Filtering, Projection, Sorting, Grouping & Aggregation");

// 5a. Filtering & Projection
var highRatedAudio = products
    .Where(p => p.Category == "Audio" && p.Rating >= 4.7)
    .Select(p => new { p.Name, p.Price, p.Rating });

Console.WriteLine("High-Rated Audio Products:");
foreach (var item in highRatedAudio)
{
    Console.WriteLine($"  • {item.Name} - Rating: {item.Rating} - {item.Price:C}");
}

// 5b. GroupBy & Aggregations
Console.WriteLine("\nCategory Statistics (GroupBy):");
var categoryStats = products
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        ProductCount = g.Count(),
        AveragePrice = g.Average(p => p.Price),
        TotalInventoryValue = g.Sum(p => p.Price * p.StockQuantity)
    })
    .OrderByDescending(stat => stat.TotalInventoryValue);

foreach (var stat in categoryStats)
{
    Console.WriteLine($"  [{stat.Category}] Count: {stat.ProductCount}, Avg Price: {stat.AveragePrice:C}, Inventory Value: {stat.TotalInventoryValue:C}");
}

// 5c. SelectMany (Flattening order items)
Console.WriteLine("\nTotal Units Sold Across All Orders (SelectMany):");
int totalItemsSold = orders
    .SelectMany(o => o.Items)
    .Sum(item => item.Quantity);
Console.WriteLine($"  Total items sold: {totalItemsSold} units");

// 5d. LINQ Join between Products and Order Items
Console.WriteLine("\nOrder Item Details with Product Names (Join):");
var orderDetails = orders
    .SelectMany(o => o.Items, (order, item) => new { order.OrderId, order.CustomerName, item })
    .Join(
        products,
        orderItem => orderItem.item.ProductId,
        product => product.Id,
        (orderItem, product) => new
        {
            orderItem.OrderId,
            orderItem.CustomerName,
            ProductName = product.Name,
            orderItem.item.Quantity,
            LineTotal = orderItem.item.Quantity * orderItem.item.UnitPrice
        }
    );

foreach (var detail in orderDetails)
{
    Console.WriteLine($"  Order #{detail.OrderId} ({detail.CustomerName}): {detail.Quantity}x {detail.ProductName} = {detail.LineTotal:C}");
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 03 Generics & LINQ completed successfully!\n");
Console.ResetColor();

static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}
