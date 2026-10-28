using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 07: DEPENDENCY INJECTION, CONFIGURATION & LOGGING");
Console.WriteLine("=========================================================\n");
Console.ResetColor();

// 1. BUILD HOST APPLICATION
var builder = Host.CreateApplicationBuilder(args);

// Ensure configuration reads appsettings.json from output directory
builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Configure Strongly-Typed Options from appsettings.json
builder.Services.Configure<PaymentGatewayOptions>(
    builder.Configuration.GetSection("PaymentGateway"));

// Register Services with Different Lifetimes
builder.Services.AddTransient<ITransientService, TransientService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddSingleton<ISingletonService, SingletonService>();

// Register Application Runner
builder.Services.AddTransient<AppRunner>();

using IHost host = builder.Build();

// Run Application Execution Demonstration
var runner = host.Services.GetRequiredService<AppRunner>();
await runner.RunAsync(host.Services);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[SUCCESS] Part 07 DI, Configuration & Logging completed successfully!\n");
Console.ResetColor();

// ============================================================================
// CONFIGURATION OPTIONS MODEL
// ============================================================================
public class PaymentGatewayOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
    public bool EnableSandbox { get; set; }
    public int RetryAttempts { get; set; }
}

// ============================================================================
// SERVICE INTERFACES & LIFETIME IMPLEMENTATIONS
// ============================================================================
public interface IOperationService
{
    Guid Id { get; }
}

public interface ITransientService : IOperationService { }
public interface IScopedService : IOperationService { }
public interface ISingletonService : IOperationService { }

public class TransientService : ITransientService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class ScopedService : IScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class SingletonService : ISingletonService
{
    public Guid Id { get; } = Guid.NewGuid();
}

// ============================================================================
// APPLICATION RUNNER
// ============================================================================
public class AppRunner
{
    private readonly PaymentGatewayOptions _gatewayOptions;
    private readonly ILogger<AppRunner> _logger;

    public AppRunner(
        IOptions<PaymentGatewayOptions> options,
        ILogger<AppRunner> logger)
    {
        _gatewayOptions = options.Value;
        _logger = logger;
    }

    public Task RunAsync(IServiceProvider rootProvider)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("--- 1. Strongly-Typed Options from Configuration ---");
        Console.ResetColor();
        string maskedKey = string.IsNullOrEmpty(_gatewayOptions.ApiKey)
            ? "N/A"
            : (_gatewayOptions.ApiKey.Length > 7 ? _gatewayOptions.ApiKey[..7] + "...[REDACTED]" : _gatewayOptions.ApiKey);
        Console.WriteLine($"  ApiKey:         {maskedKey}");
        Console.WriteLine($"  Sandbox Mode:   {_gatewayOptions.EnableSandbox}");
        Console.WriteLine($"  Timeout:        {_gatewayOptions.TimeoutSeconds}s");
        Console.WriteLine($"  Retry Attempts: {_gatewayOptions.RetryAttempts}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- 2. Structured Logging with ILogger ---");
        Console.ResetColor();
        string orderId = "ORD-99812";
        decimal orderTotal = 499.95m;
        // Structured logging template (NOT string interpolation!)
        _logger.LogInformation("Processing checkout for OrderId: {OrderId} with Total: {OrderTotal:C}", orderId, orderTotal);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- 3. Dependency Injection Service Lifetimes ---");
        Console.ResetColor();

        // Scope 1
        Console.WriteLine("Creating Scope 1...");
        using (var scope1 = rootProvider.CreateScope())
        {
            var t1 = scope1.ServiceProvider.GetRequiredService<ITransientService>();
            var t2 = scope1.ServiceProvider.GetRequiredService<ITransientService>();
            var s1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
            var s2 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
            var single1 = scope1.ServiceProvider.GetRequiredService<ISingletonService>();

            Console.WriteLine($"  [Scope 1] Transient 1:  {t1.Id}");
            Console.WriteLine($"  [Scope 1] Transient 2:  {t2.Id} (DIFFERENT -> new per call)");
            Console.WriteLine($"  [Scope 1] Scoped 1:     {s1.Id}");
            Console.WriteLine($"  [Scope 1] Scoped 2:     {s2.Id} (SAME within scope!)");
            Console.WriteLine($"  [Scope 1] Singleton:    {single1.Id}");
        }

        // Scope 2
        Console.WriteLine("\nCreating Scope 2...");
        using (var scope2 = rootProvider.CreateScope())
        {
            var s1 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
            var single2 = scope2.ServiceProvider.GetRequiredService<ISingletonService>();

            Console.WriteLine($"  [Scope 2] Scoped:       {s1.Id} (DIFFERENT -> new per scope)");
            Console.WriteLine($"  [Scope 2] Singleton:    {single2.Id} (SAME across all scopes!)");
        }

        return Task.CompletedTask;
    }
}
