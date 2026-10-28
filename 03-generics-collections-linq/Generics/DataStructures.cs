namespace GenericsAndLinq.Generics;

using GenericsAndLinq.Models;

/// <summary>
/// Generic in-memory repository demonstrating type constraints:
/// - TEntity must be a reference type ('class')
/// - TEntity must implement IEntity<TId>
/// - TId must be non-null ('notnull') and comparable
/// </summary>
public class InMemoryRepository<TId, TEntity>
    where TId : notnull, IComparable<TId>
    where TEntity : class, IEntity<TId>
{
    private readonly Dictionary<TId, TEntity> _storage = [];

    public void AddOrUpdate(TEntity entity)
    {
        _storage[entity.Id] = entity;
    }

    public TEntity? GetById(TId id)
    {
        return _storage.TryGetValue(id, out var entity) ? entity : null;
    }

    public IEnumerable<TEntity> GetAll() => _storage.Values;

    public int Count => _storage.Count;
}

/// <summary>
/// Generic utility demonstrating generic method constraints.
/// </summary>
public static class GenericUtils
{
    // Generic method with IComparable constraint
    public static T MaxOf<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) >= 0 ? a : b;
    }

    // Generic swap method using tuple assignment
    public static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
}
