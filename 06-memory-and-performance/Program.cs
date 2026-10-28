using System.Buffers;
using System.Diagnostics;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 06: MEMORY MANAGEMENT, PERFORMANCE & SPAN<T>");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// ----------------------------------------------------------------------------
// 1. GARBAGE COLLECTION INSPECTION & GENERATIONS
// ----------------------------------------------------------------------------
PrintHeader("1. Garbage Collector Internals (Generations 0, 1, 2)");

long memoryBefore = GC.GetTotalMemory(forceFullCollection: false);
Console.WriteLine($"[GC Status] Initial Allocated Memory: {memoryBefore:N0} bytes");

// Allocate short-lived objects
for (int i = 0; i < 50_000; i++)
{
    var temp = new byte[100];
}

long memoryAfter = GC.GetTotalMemory(false);
Console.WriteLine($"[GC Status] Memory after 50,000 allocations: {memoryAfter:N0} bytes");
Console.WriteLine($"[GC Collections] Gen 0: {GC.CollectionCount(0)}, Gen 1: {GC.CollectionCount(1)}, Gen 2: {GC.CollectionCount(2)}");

// Force a collection to observe cleanup
GC.Collect();
GC.WaitForPendingFinalizers();
Console.WriteLine($"[GC Status] Memory after GC.Collect(): {GC.GetTotalMemory(false):N0} bytes");

// ----------------------------------------------------------------------------
// 2. IDisposable & DETERMINISTIC CLEANUP
// ----------------------------------------------------------------------------
PrintHeader("2. Deterministic Cleanup with IDisposable");

using (var resource = new ManagedResourceWrapper("DatabaseConnectionPool"))
{
    resource.PerformWork();
} // Automatically disposed at end of scope!

// ----------------------------------------------------------------------------
// 3. ZERO-ALLOCATION PARSING WITH ReadOnlySpan<char>
// ----------------------------------------------------------------------------
PrintHeader("3. Zero-Allocation String Parsing with ReadOnlySpan<char>");

string logLine = "2026-10-28 09:30:15 [ERROR] ServiceOrder: Payment timeout for ID:884920";

// Traditional Substring allocation vs Span Slicing
ReadOnlySpan<char> span = logLine.AsSpan();

// Extract Date without allocating a new string
ReadOnlySpan<char> dateSpan = span[..10];

// Extract Log Level without allocating
int levelStart = span.IndexOf('[') + 1;
int levelEnd = span.IndexOf(']');
ReadOnlySpan<char> levelSpan = span[levelStart..levelEnd];

// Extract ID
int idPrefixIndex = span.IndexOf("ID:", StringComparison.Ordinal) + 3;
ReadOnlySpan<char> idSpan = span[idPrefixIndex..];

Console.WriteLine($"[Span Slicing] Extracted Date:       \"{dateSpan}\" (Zero heap allocation!)");
Console.WriteLine($"[Span Slicing] Extracted Level:      \"{levelSpan}\"");
Console.WriteLine($"[Span Slicing] Extracted Target ID:  \"{idSpan}\"");

// Parsing numbers directly from ReadOnlySpan<char> without string allocation
if (int.TryParse(idSpan, out int parsedId))
{
    Console.WriteLine($"[Span Parsing] Parsed Integer:       {parsedId}");
}

// ----------------------------------------------------------------------------
// 4. HIGH-PERFORMANCE BUFFER POOLING WITH ArrayPool<T>
// ----------------------------------------------------------------------------
PrintHeader("4. Buffer Reuse with ArrayPool<T>.Shared");

// Rent a buffer from the shared pool
int bufferSize = 1024 * 16; // 16 KB
byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);

try
{
    Console.WriteLine($"[ArrayPool] Rented buffer length: {rentedBuffer.Length} bytes (reused from pool)");
    // Fill buffer
    rentedBuffer.AsSpan().Fill(0xFF);
    Console.WriteLine($"[ArrayPool] First byte: 0x{rentedBuffer[0]:X2}, Last byte: 0x{rentedBuffer[^1]:X2}");
}
finally
{
    // Always return buffer in a finally block to prevent pool leaks!
    ArrayPool<byte>.Shared.Return(rentedBuffer, clearArray: true);
    Console.WriteLine("[ArrayPool] Successfully returned buffer to pool.");
}

// ----------------------------------------------------------------------------
// 5. STACKALLOC FOR ZERO-HEAP ALLOCATION
// ----------------------------------------------------------------------------
PrintHeader("5. Stack Allocation (stackalloc)");

// Allocate a small buffer directly on the thread stack
Span<int> stackNumbers = stackalloc int[5];
for (int i = 0; i < stackNumbers.Length; i++)
{
    stackNumbers[i] = (i + 1) * 10;
}

Console.Write("[stackalloc buffer]: ");
foreach (int val in stackNumbers)
{
    Console.Write($"{val} ");
}
Console.WriteLine("(Lives purely on the Stack, zero GC involvement!)");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 06 Memory Management & Span completed successfully!\n");
Console.ResetColor();

// ============================================================================
// HELPER MODELS & CLASSES
// ============================================================================
static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}

public class ManagedResourceWrapper : IDisposable
{
    public string ResourceName { get; }
    private bool _disposed;

    public ManagedResourceWrapper(string name)
    {
        ResourceName = name;
        Console.WriteLine($"  [Resource] Opened: {ResourceName}");
    }

    public void PerformWork()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Console.WriteLine($"  [Resource] Performing active work on: {ResourceName}...");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevent GC finalizer queue overhead
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Free managed resources
                Console.WriteLine($"  [Resource] Freeing managed resources for: {ResourceName}");
            }
            _disposed = true;
        }
    }
}
