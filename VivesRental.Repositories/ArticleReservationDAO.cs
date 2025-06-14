using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VivesRental.Domains.DataDB;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Repositories.Interfaces;

namespace VivesRental.Repositories
{
    public class ArticleReservationDAO : IDAO<ArticleReservation>
    {
        private readonly RentalDbContext _context;

        public ArticleReservationDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ArticleReservation entity)
        {
            try
            {
                await _context.ArticleReservations.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(ArticleReservation entity)
        {
            try
            {
                _context.ArticleReservations.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<ArticleReservation?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.ArticleReservations.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ArticleReservation>?> GetAllAsync()
        {
            try
            {
                return await _context.ArticleReservations
                    .Include(ar => ar.Article)
                        .ThenInclude(a => a.Product)
                    .Include(ar => ar.Customer)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }


        public async Task UpdateAsync(ArticleReservation entity)
        {
            try
            {
                _context.ArticleReservations.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }
    }
}
