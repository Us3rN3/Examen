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
    public class ProductService : IService<Product>
    {
        private readonly IDAO<Product> _productDAO;
        public ProductService(IDAO<Product> productDAO)
        {
            _productDAO = productDAO;
        }
        public Task AddAsync(Product entity)
        {
            try 
            {
                return _productDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public Task DeleteAsync(Product entity)
        {
            try 
            {
                return _productDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public Task<Product?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _productDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<Product?>(null);
            }
        }

        public Task<IEnumerable<Product>?> GetAllAsync()
        {
            try 
            {
                return _productDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<Product>?>(null);
            }
        }

        public Task UpdateAsync(Product entity)
        {
            try 
            {
                return _productDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    }
}
