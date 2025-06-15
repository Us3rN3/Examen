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
    public class OrderLineService : IService<OrderLine>
    {
        private readonly IDAO<OrderLine> _orderLineDAO;
        public OrderLineService(IDAO<OrderLine> orderLineDAO)
        {
            _orderLineDAO = orderLineDAO;
        }

        public Task AddAsync(OrderLine entity)
        {
            try 
            {
                return _orderLineDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public Task DeleteAsync(OrderLine entity)
        {
            try 
            {
                return _orderLineDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public Task<OrderLine?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _orderLineDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<OrderLine?>(null);
            }
        }

        public Task<IEnumerable<OrderLine>?> GetAllAsync()
        {
            try 
            {
                return _orderLineDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<OrderLine>?>(null);
            }
        }

        public Task UpdateAsync(OrderLine entity)
        {
            try 
            {
                return _orderLineDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    
    }
}
