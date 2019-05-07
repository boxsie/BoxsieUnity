using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boxsie.Network.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        bool UseCollection { get; set; }

        T Get(Guid id);
        List<T> GetAll();
        void Update(T obj);
        void Update(Guid itemId, byte[] bytes);
        void Delete(Guid id);
    }

    public interface IRepositoryAsync<T>
    {
        bool UseCollection { get; set; }

        Task<T> GetAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task UpdateAsync(T obj);
        Task UpdateAsync(Guid itemId, byte[] bytes);
        Task DeleteAsync(Guid id);
    }
}