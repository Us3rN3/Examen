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
    public class CustomerService : IService<Customer>
    {
        private readonly IDAO<Customer> _customerDAO;
        public CustomerService(IDAO<Customer> customerDAO)
        {
            _customerDAO = customerDAO;
        }
        public Task AddAsync(Customer entity)
        {
            try 
            {
                return _customerDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public Task DeleteAsync(Customer entity)
        {
            try 
            {
                return _customerDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public Task<Customer?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _customerDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<Customer?>(null);
            }
        }

        public Task<IEnumerable<Customer>?> GetAllAsync()
        {
            try 
            {
                return _customerDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<Customer>?>(null);
            }
        }

        public Task UpdateAsync(Customer entity)
        {
            try 
            {
                return _customerDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    }
}
