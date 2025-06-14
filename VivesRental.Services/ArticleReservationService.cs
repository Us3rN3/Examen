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
    public class ArticleReservationService : IService<ArticleReservation>
    {
        private readonly IDAO<ArticleReservation> _articleReservationDAO;
        public ArticleReservationService(IDAO<ArticleReservation> articleReservationDAO)
        {
            _articleReservationDAO = articleReservationDAO;
        }
        public Task AddAsync(ArticleReservation entity)
        {
            try 
            {
                return _articleReservationDAO.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }   
        }

        public Task DeleteAsync(ArticleReservation entity)
        {
            try 
            {
                return _articleReservationDAO.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }   
        }

        public Task<ArticleReservation?> FindByIdAsync(Guid id)
        {
            try 
            {
                return _articleReservationDAO.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return Task.FromResult<ArticleReservation?>(null);
            }   
        }

        public Task<IEnumerable<ArticleReservation>?> GetAllAsync()
        {
            try
            {
                return _articleReservationDAO.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return Task.FromResult<IEnumerable<ArticleReservation>?>(null);
            }
        }

        public Task UpdateAsync(ArticleReservation entity)
        {
            try 
            {
                return _articleReservationDAO.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
    }

    }
}