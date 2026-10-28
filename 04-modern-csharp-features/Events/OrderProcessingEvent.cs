namespace ModernCSharp.Events;

public class OrderCompletedEventArgs : EventArgs
{
    public required string OrderNumber { get; init; }
    public required decimal Amount { get; init; }
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
}

public class OrderService
{
    // C# idiomatic event declaration
    public event EventHandler<OrderCompletedEventArgs>? OrderCompleted;

    public void CompleteOrder(string orderNumber, decimal amount)
    {
        Console.WriteLine($"[OrderService] Completing order #{orderNumber} for {amount:C}...");

        // Fire event safely using null-conditional invoke
        OrderCompleted?.Invoke(this, new OrderCompletedEventArgs
        {
            OrderNumber = orderNumber,
            Amount = amount
        });
    }
}
