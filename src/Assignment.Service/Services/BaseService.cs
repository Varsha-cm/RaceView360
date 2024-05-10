using Assignment.Core.Interfaces;

namespace Assignment.Service
{
    public class BaseService<T> where T : class
    {
        private readonly IDBGenericRepository<T> _repository;

        public BaseService(IDBGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> GetAsync(int? id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> Exists(int id)
        {
            return await _repository.Exists(id);
        }
    }
}
