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
    public class ArticleService : IService<Article>
    {
        private readonly IDAO<Article> _articleDAO;

        public ArticleService(IDAO<Article> articleDao)
        {
            _articleDAO = articleDao;
        }
        public Task AddAsync(Article entity)
        {
            try 
            {
                return _articleDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public Task DeleteAsync(Article entity)
        {
            try 
            {
                return _articleDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public Task<Article?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _articleDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<Article?>(null);
            }
        }

        public Task<IEnumerable<Article>?> GetAllAsync()
        {
            try 
            {
                return _articleDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<Article>?>(null);
            }
        }

        public Task UpdateAsync(Article entity)
        {
            try 
            {
                return _articleDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    }
}
