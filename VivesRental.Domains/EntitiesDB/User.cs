namespace VivesRental.Domains.EntitiesDB
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }  // Hashen in productie!
        public string Role { get; set; }
    }
}
