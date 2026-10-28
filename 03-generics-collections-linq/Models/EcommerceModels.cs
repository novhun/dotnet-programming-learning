namespace GenericsAndLinq.Models;

public interface IEntity<TId>
{
    TId Id { get; }
}

public record Product(
    int Id,
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    double Rating
) : IEntity<int>;

public record OrderItem(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

public record CustomerOrder(
    int OrderId,
    string CustomerName,
    DateTime OrderDate,
    List<OrderItem> Items,
    bool IsDelivered
) : IEntity<int>
{
    public int Id => OrderId;
}
