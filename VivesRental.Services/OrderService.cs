using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Repositories.Interfaces;
using VivesRental.Services.Interfaces;

namespace VivesRental.Services
{
    public class OrderService : IService<Order>
    {
        private readonly IDAO<Order> _orderDAO;
        public OrderService(IDAO<Order> orderDAO)
        {
            _orderDAO = orderDAO;
        }
        public Task AddAsync(Order entity)
        {
            try 
            {
                return _orderDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public Task DeleteAsync(Order entity)
        {
            try 
            {
                return _orderDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public Task<Order?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _orderDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<Order?>(null);
            }
        }

        public Task<IEnumerable<Order>?> GetAllAsync()
        {
            try 
            {
                return _orderDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<Order>?>(null);
            }
        }

        public Task UpdateAsync(Order entity)
        {
            try 
            {
                return _orderDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    }
}
