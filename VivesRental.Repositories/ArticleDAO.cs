using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VivesRental.Domains.DataDB;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VivesRental.Repositories
{
    public class ArticleDAO : IDAO<Article>
    {
        private readonly RentalDbContext _context;

        public ArticleDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Article entity)
        {
            try
            {
                await _context.Articles.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Article entity)
        {
            try
            {
                _context.Articles.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Article?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Articles
                .Include(a => a.Product) // <-- Ook hier toevoegen voor Details/Edit views
                .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Article>?> GetAllAsync()
        {
            try
            {
                return await _context.Articles
                .Include(a => a.Product) // <-- Belangrijk voor weergave in view
                .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(Article entity)
        {
            try
            {
                var existingArticle = await _context.Articles.FindAsync(entity.Id);
                if (existingArticle == null)
                    throw new Exception("Artikel niet gevonden");

                existingArticle.Status = entity.Status;
                // Andere velden toevoegen indien nodig

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
