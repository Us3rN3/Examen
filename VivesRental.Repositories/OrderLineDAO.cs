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
    public class OrderLineDAO : IDAO<OrderLine>
    {
        private readonly RentalDbContext _context;

        public OrderLineDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OrderLine entity)
        {
            try
            {
                await _context.OrderLines.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(OrderLine entity)
        {
            try
            {
                _context.OrderLines.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OrderLine?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.OrderLines.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OrderLine>?> GetAllAsync()
        {
            try
            {
                return await _context.OrderLines.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(OrderLine entity)
        {
            try
            {
                _context.OrderLines.Update(entity);
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
