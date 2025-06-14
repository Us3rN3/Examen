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
    public class CustomerDAO : IDAO<Customer>
    {
        private readonly RentalDbContext _context;

        public CustomerDAO(RentalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer entity)
        {
            try
            {
                await _context.Customers.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Customer entity)
        {
            try
            {
                _context.Customers.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Customer?> FindByIdAsync(Guid id)
        {
            try
            {
                return await _context.Customers.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Customer>?> GetAllAsync()
        {
            try
            {
                return await _context.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(Customer entity)
        {
            try
            {
                _context.Customers.Update(entity);
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
