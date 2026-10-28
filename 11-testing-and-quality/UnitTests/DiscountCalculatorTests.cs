namespace TestingAndQuality.UnitTests;

using TestingAndQuality.Services;
using Xunit;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = new(); // SUT = System Under Test

    [Theory]
    [InlineData(100.0, "Bronze", 0.05)]
    [InlineData(100.0, "Silver", 0.10)]
    [InlineData(100.0, "Gold", 0.15)]
    [InlineData(100.0, "Unknown", 0.0)]
    public void CalculateDiscount_StandardTiers_ReturnsExpectedRate(
        decimal totalAmount, string tier, decimal expectedRate)
    {
        // Act
        decimal result = _sut.CalculateDiscount(totalAmount, tier);

        // Assert
        Assert.Equal(expectedRate, result);
    }

    [Fact]
    public void CalculateDiscount_PlatinumHighValue_ReturnsSuperDiscount()
    {
        // Arrange
        decimal amount = 600.00m;
        string tier = "Platinum";

        // Act
        decimal rate = _sut.CalculateDiscount(amount, tier);

        // Assert
        Assert.Equal(0.25m, rate);
    }

    [Fact]
    public void CalculateDiscount_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        decimal invalidAmount = -50.00m;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _sut.CalculateDiscount(invalidAmount, "Gold");
        });
    }
}
