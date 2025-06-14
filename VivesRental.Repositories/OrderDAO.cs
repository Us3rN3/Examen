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
    public class OrderDAO : IDAO<Order>
    {
        private readonly RentalDbContext _context;

        public OrderDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order entity)
        {
            try
            {
                await _context.Orders.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Order entity)
        {
            try
            {
                _context.Orders.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Order?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Order>?> GetAllAsync()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(Order entity)
        {
            try
            {
                _context.Orders.Update(entity);
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
