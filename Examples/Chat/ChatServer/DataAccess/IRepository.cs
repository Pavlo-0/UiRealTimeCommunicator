namespace Chat.DataAccess
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Remove(T entity);
        T? GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}
