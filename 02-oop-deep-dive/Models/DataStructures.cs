namespace OopDeepDive.Models;

/// <summary>
/// Value-type struct: Ideal for small, immutable data structures with value equality.
/// Lives on stack unless boxed or contained inside a reference type.
/// </summary>
public readonly struct GeoPoint
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString() => $"GeoPoint({Latitude:F4}, {Longitude:F4})";
}

/// <summary>
/// Positional Record: Reference type with built-in value-based equality,
/// auto-generated ToString(), hashcode, deconstructor, and non-destructive mutation (`with`).
/// </summary>
public record Customer(int Id, string FullName, string Email, string Tier);

/// <summary>
/// Record Struct: Value type with record features (value equality, with-expression)
/// </summary>
public readonly record struct Money(decimal Amount, string Currency)
{
    public override string ToString() => $"{Amount:N2} {Currency}";
}

/// <summary>
/// Standard Class for reference equality comparison.
/// </summary>
public class StandardCustomerClass
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;

    public StandardCustomerClass(int id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }
}
