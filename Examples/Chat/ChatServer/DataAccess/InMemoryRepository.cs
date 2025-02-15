namespace Chat.DataAccess
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _store = new();

        public void Add(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new InvalidOperationException("Entity must have an 'Id' property of type Guid.");
            }

            var id = (Guid)idProperty.GetValue(entity)!;
            _store[id] = entity;
        }

        public void Remove(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new InvalidOperationException("Entity must have an 'Id' property of type Guid.");
            }

            var id = (Guid)idProperty.GetValue(entity)!;
            _store.Remove(id);
        }

        public T? GetById(Guid id) => _store.ContainsKey(id) ? _store[id] : null;

        public IEnumerable<T> GetAll() => _store.Values.ToList();
    }

}
