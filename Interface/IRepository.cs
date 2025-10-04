namespace QuanLi_CF.Interface;
public interface IRepository<T,TKey>
{
    T GetById(TKey id);
    bool TryGetById(TKey id, out T entity);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
}
