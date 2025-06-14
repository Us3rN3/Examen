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
    public class ProductDAO : IDAO<Product>
    {
        private readonly RentalDbContext _context;

        public ProductDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product entity)
        {
            try
            {
                if (entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                }
                await _context.Products.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Product entity)
        {
            try
            {
                _context.Products.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Product?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Product>?> GetAllAsync()
        {
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(Product entity)
        {
            try
            {
                _context.Products.Update(entity);
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
