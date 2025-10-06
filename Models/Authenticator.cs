using System.Collections.Generic;
using System.Linq;

namespace YourApp.Models
{
    public class Authenticator
    {
        private readonly List<User> _users;

        public Authenticator()
        {
            // Simulated user database — replace with DB or API call in production
            _users = new List<User>
            {
                new User { UserID = "1", Username = "admin", Password = "password" },
                new User { UserID = "2", Username = "user", Password = "1234" }
            };
        }

        public User Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool Authorise(User user)
        {
            // Simple placeholder: allow all authenticated users
            return user != null;
        }
    }
}