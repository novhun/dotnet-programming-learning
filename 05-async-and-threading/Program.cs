using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 05: ASYNCHRONOUS PROGRAMMING & CONCURRENCY");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. CONCURRENT TASK EXECUTION (Task.WhenAll)
// ----------------------------------------------------------------------------
PrintHeader("1. Parallel Async Operations with Task.WhenAll");

var stopwatch = Stopwatch.StartNew();
var taskA = FetchDataAsync("User Service", delayMs: 400);
var taskB = FetchDataAsync("Order Service", delayMs: 300);
var taskC = FetchDataAsync("Payment Service", delayMs: 500);

string[] results = await Task.WhenAll(taskA, taskB, taskC);
stopwatch.Stop();

foreach (var res in results)
{
    Console.WriteLine($"  -> Result: {res}");
}
Console.WriteLine($"[Execution Time] Total elapsed: {stopwatch.ElapsedMilliseconds}ms (Ran concurrently, NOT sequentially!)");

// ----------------------------------------------------------------------------
// 2. ValueTask<T> FOR HIGH PERFORMANCE SYNCHRONOUS PATHS
// ----------------------------------------------------------------------------
PrintHeader("2. ValueTask<T> Optimization (Avoiding Allocations)");

var cacheService = new FastCacheService();
// First call: async cache miss
string val1 = await cacheService.GetCachedDataAsync("key-1");
// Second call: immediate synchronous cache hit (zero Task allocation!)
string val2 = await cacheService.GetCachedDataAsync("key-1");
Console.WriteLine($"[ValueTask] First call: {val1}");
Console.WriteLine($"[ValueTask] Second call: {val2}");

// ----------------------------------------------------------------------------
// 3. CANCELLATION TOKENS & TIMEOUTS
// ----------------------------------------------------------------------------
PrintHeader("3. Cooperative Cancellation with CancellationToken");

using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350)); // Timeout after 350ms
try
{
    Console.WriteLine("[Cancellation] Starting long running job with 350ms timeout...");
    await LongRunningJobAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("[Cancellation] Job was safely cancelled as expected due to timeout!");
}

// ----------------------------------------------------------------------------
// 4. RESOURCE THROTTLING WITH SemaphoreSlim
// ----------------------------------------------------------------------------
PrintHeader("4. Concurrency Throttling with SemaphoreSlim (Max 2 concurrent)");

var throttler = new SemaphoreSlim(initialCount: 2, maxCount: 2);
List<Task> jobs = [];

for (int i = 1; i <= 5; i++)
{
    int jobId = i;
    jobs.Add(Task.Run(async () =>
    {
        await throttler.WaitAsync();
        try
        {
            Console.WriteLine($"  [Worker {jobId}] Entered critical section (Thread {Environment.CurrentManagedThreadId})");
            await Task.Delay(200); // Simulate work
        }
        finally
        {
            Console.WriteLine($"  [Worker {jobId}] Leaving critical section.");
            throttler.Release();
        }
    }));
}

await Task.WhenAll(jobs);

// ----------------------------------------------------------------------------
// 5. LOCK-FREE ATOMICS WITH Interlocked
// ----------------------------------------------------------------------------
PrintHeader("5. Lock-free Atomic Counters with Interlocked");

int sharedCounter = 0;
List<Task> counterTasks = [];

for (int i = 0; i < 100; i++)
{
    counterTasks.Add(Task.Run(() =>
    {
        for (int j = 0; j < 1000; j++)
        {
            Interlocked.Increment(ref sharedCounter);
        }
    }));
}

await Task.WhenAll(counterTasks);
Console.WriteLine($"[Interlocked Counter] Expected: 100,000 | Actual: {sharedCounter:N0}");

// ----------------------------------------------------------------------------
// 6. ASYNC STREAMS (IAsyncEnumerable<T>)
// ----------------------------------------------------------------------------
PrintHeader("6. Asynchronous Streams (IAsyncEnumerable<T>)");

Console.WriteLine("[Streaming Stock Ticks]:");
await foreach (var tick in StreamStockPricesAsync("MSFT", 5))
{
    Console.WriteLine($"  -> Stream received: {tick.Symbol} = {tick.Price:C} at {tick.Timestamp:HH:mm:ss.fff}");
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 05 Asynchronous Programming completed successfully!\n");
Console.ResetColor();

// ============================================================================
// HELPER METHODS & CLASSES
// ============================================================================
static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}

static async Task<string> FetchDataAsync(string serviceName, int delayMs)
{
    await Task.Delay(delayMs);
    return $"{serviceName} responded in {delayMs}ms";
}

static async Task LongRunningJobAsync(CancellationToken token)
{
    for (int step = 1; step <= 10; step++)
    {
        token.ThrowIfCancellationRequested();
        Console.WriteLine($"  -> Step {step}/10 running...");
        await Task.Delay(100, token);
    }
}

static async IAsyncEnumerable<StockTick> StreamStockPricesAsync(string symbol, int count, [EnumeratorCancellation] CancellationToken token = default)
{
    var random = new Random();
    decimal currentPrice = 420.50m;

    for (int i = 0; i < count; i++)
    {
        token.ThrowIfCancellationRequested();
        await Task.Delay(150, token); // Simulate waiting for network packet
        currentPrice += (decimal)(random.NextDouble() * 4 - 2);
        yield return new StockTick(symbol, Math.Round(currentPrice, 2), DateTime.UtcNow);
    }
}

public record StockTick(string Symbol, decimal Price, DateTime Timestamp);

public class FastCacheService
{
    private readonly Dictionary<string, string> _memoryCache = [];

    public ValueTask<string> GetCachedDataAsync(string key)
    {
        // Synchronous hot-path: Zero Task allocation!
        if (_memoryCache.TryGetValue(key, out var cachedVal))
        {
            return new ValueTask<string>($"[CACHE HIT] {cachedVal}");
        }

        // Asynchronous slow-path
        return new ValueTask<string>(FetchAndCacheAsync(key));
    }

    private async Task<string> FetchAndCacheAsync(string key)
    {
        await Task.Delay(200); // Simulate network fetch
        string fetchedData = $"Data-for-{key}-{Guid.NewGuid().ToString()[..6]}";
        _memoryCache[key] = fetchedData;
        return $"[FETCHED] {fetchedData}";
    }
}
