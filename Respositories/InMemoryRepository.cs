using QuanLi_CF.Interface;
namespace QuanLi_CF.Repositories;

public class InMemoryRepository<T, TKey> : IRepository<T, TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, T> store = new();
    private readonly Func<T, TKey> keySelector;
    public InMemoryRepository(Func<T, TKey> keySelector)
    {
        this.keySelector = keySelector;
    }
    public void Add(T entity) => store[keySelector(entity)] = entity;
    public T GetById(TKey id)
    {
        if (!store.ContainsKey(id))
            throw new KeyNotFoundException($"Khong the tim thay {id}");
        return store[id];
    }
    public bool TryGetById(TKey id, out T entity)
    {
        return store.TryGetValue(id, out entity!);
    }
    public IEnumerable<T> GetAll()
    {
        return store.Values;
    }
    public void Update(T entity)
    {
        if (!store.ContainsKey(keySelector(entity)))
        {
            throw new KeyNotFoundException($"Khong the tim thay {keySelector(entity)}");
        }

        store[keySelector(entity)] = entity;
    }
    
}