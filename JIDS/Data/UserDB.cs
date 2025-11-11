namespace JIDS.Data;
using System.ComponentModel.DataAnnotations;

public class UserDB
{
    [Key]
    public Guid UserID { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty; 

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<JetConfigurationDB>? Configurations { get; set; }
}