using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 
using JetInteriorApp.Models; 

namespace JetInteriorApp.Models
{
    public class JetConfiguration
    {
        [Key]
        public Guid ConfigID { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        public string? Name { get; set; }
        public string? CabinDimensions { get; set; }
        public int SeatingCapacity { get; set; }
        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<InteriorComponent>? InteriorComponents { get; set; }

    }
}