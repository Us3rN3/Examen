using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivesRental.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        // Algemene CRUD-operaties
        Task<IEnumerable<T>?> GetAllAsync();
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> FindByIdAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}
