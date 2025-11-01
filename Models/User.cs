using System.ComponentModel.DataAnnotations;
using JetInteriorApp.Models; 

namespace JetInteriorApp.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<JetConfiguration> Configurations { get; set; }
    }
}