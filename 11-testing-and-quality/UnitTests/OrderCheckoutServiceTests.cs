namespace TestingAndQuality.UnitTests;

using TestingAndQuality.Services;
using Xunit;

public class OrderCheckoutServiceTests
{
    [Fact]
    public async Task CheckoutAsync_WhenStockAvailableAndPaymentSucceeds_ReturnsTrue()
    {
        // Arrange
        var mockInventory = new FakeInventoryService(shouldSucceed: true);
        var mockPayment = new FakePaymentGateway(shouldSucceed: true);
        var calculator = new DiscountCalculator();

        var sut = new OrderCheckoutService(mockPayment, mockInventory, calculator);

        // Act
        bool result = await sut.CheckoutAsync("CUST-1", "Gold", productId: 10, quantity: 2, unitPrice: 50m);

        // Assert
        Assert.True(result);
        Assert.True(mockPayment.ChargeWasCalled);
        Assert.Equal(85m, mockPayment.LastAmountCharged); // 2 * $50 = $100 - 15% ($15) = $85
    }

    [Fact]
    public async Task CheckoutAsync_WhenInventoryFails_NeverChargesPaymentGateway()
    {
        // Arrange
        var mockInventory = new FakeInventoryService(shouldSucceed: false); // Out of stock
        var mockPayment = new FakePaymentGateway(shouldSucceed: true);
        var calculator = new DiscountCalculator();

        var sut = new OrderCheckoutService(mockPayment, mockInventory, calculator);

        // Act
        bool result = await sut.CheckoutAsync("CUST-2", "Silver", productId: 10, quantity: 5, unitPrice: 20m);

        // Assert
        Assert.False(result);
        Assert.False(mockPayment.ChargeWasCalled); // Critical: customer must NOT be charged if out of stock!
    }

    [Fact]
    public async Task CheckoutAsync_InvalidQuantity_ThrowsArgumentException()
    {
        // Arrange
        var mockInventory = new FakeInventoryService(shouldSucceed: true);
        var mockPayment = new FakePaymentGateway(shouldSucceed: true);
        var calculator = new DiscountCalculator();

        var sut = new OrderCheckoutService(mockPayment, mockInventory, calculator);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.CheckoutAsync("CUST-3", "Bronze", productId: 1, quantity: 0, unitPrice: 10m));
    }
}

// ----------------------------------------------------------------------------
// TEST DOUBLES / MOCKS
// ----------------------------------------------------------------------------
public class FakeInventoryService : IInventoryService
{
    private readonly bool _shouldSucceed;

    public FakeInventoryService(bool shouldSucceed)
    {
        _shouldSucceed = shouldSucceed;
    }

    public Task<bool> ReserveStockAsync(int productId, int quantity)
    {
        return Task.FromResult(_shouldSucceed);
    }
}

public class FakePaymentGateway : IPaymentGateway
{
    private readonly bool _shouldSucceed;
    public bool ChargeWasCalled { get; private set; }
    public decimal LastAmountCharged { get; private set; }

    public FakePaymentGateway(bool shouldSucceed)
    {
        _shouldSucceed = shouldSucceed;
    }

    public Task<bool> ChargeAsync(string customerId, decimal amount)
    {
        ChargeWasCalled = true;
        LastAmountCharged = amount;
        return Task.FromResult(_shouldSucceed);
    }
}
