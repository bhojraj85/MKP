using KTLearningPlatform.Core.Interfaces;

namespace KTLearningPlatform.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public Task<T?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<T>> GetAllAsync() => throw new NotImplementedException();
        public Task AddAsync(T entity) => throw new NotImplementedException();
        public void Update(T entity) => throw new NotImplementedException();
        public void Delete(T entity) => throw new NotImplementedException();
        public Task<int> SaveChangesAsync() => throw new NotImplementedException();
    }
}