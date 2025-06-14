using VivesRental.Domains.EntitiesDB;

public interface IUserService
{
    User? ValidateUser(string username, string password);
}

public class UserService : IUserService
{
    private static readonly List<User> _users = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "Admin" },
        new User { Username = "medewerker", Password = "wachtwoord", Role = "User" }
    };

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password); // In real: hash wachtwoord vergelijken!
    }
}

