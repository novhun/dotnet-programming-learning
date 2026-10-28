namespace TestingAndQuality.Services;

public interface IPaymentGateway
{
    Task<bool> ChargeAsync(string customerId, decimal amount);
}

public interface IInventoryService
{
    Task<bool> ReserveStockAsync(int productId, int quantity);
}

public class DiscountCalculator
{
    public decimal CalculateDiscount(decimal totalAmount, string customerTier)
    {
        if (totalAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount cannot be negative.");

        return customerTier.ToUpperInvariant() switch
        {
            "PLATINUM" => totalAmount >= 500m ? 0.25m : 0.20m, // 25% or 20%
            "GOLD" => 0.15m,
            "SILVER" => 0.10m,
            "BRONZE" => 0.05m,
            _ => 0.0m
        };
    }
}

public class OrderCheckoutService
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IInventoryService _inventoryService;
    private readonly DiscountCalculator _discountCalculator;

    public OrderCheckoutService(
        IPaymentGateway paymentGateway,
        IInventoryService inventoryService,
        DiscountCalculator discountCalculator)
    {
        _paymentGateway = paymentGateway;
        _inventoryService = inventoryService;
        _discountCalculator = discountCalculator;
    }

    public async Task<bool> CheckoutAsync(string customerId, string customerTier, int productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        // 1. Reserve stock
        bool stockReserved = await _inventoryService.ReserveStockAsync(productId, quantity);
        if (!stockReserved)
            return false;

        // 2. Calculate discounted total
        decimal rawTotal = quantity * unitPrice;
        decimal discountRate = _discountCalculator.CalculateDiscount(rawTotal, customerTier);
        decimal finalAmount = rawTotal * (1m - discountRate);

        // 3. Process payment
        return await _paymentGateway.ChargeAsync(customerId, finalAmount);
    }
}
