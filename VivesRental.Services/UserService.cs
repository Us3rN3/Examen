// UserService.cs
using VivesRental.Domains.EntitiesDB;
using VivesRental.Services.Interfaces;

namespace VivesRental.Services
{
    public class UserService : IUserService
    {
        private static readonly List<User> _users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "Administrator" },
            new User { Username = "user", Password = "user123", Role = "User" }
        };

        public User? ValidateUser(string username, string password)
        {
            return _users.SingleOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);
        }
    }
}
