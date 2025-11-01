using System.ComponentModel.DataAnnotations.Schema;

namespace JetInteriorApp.Models
{
    public class JetConfiguration
    {
        [Key]
        public int ConfigID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public string ModelName { get; set; }
        public string CabinDimensions { get; set; }
        public int SeatingCapacity { get; set; }
        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string ConfigJson { get; set; }

        public User User { get; set; }
        public ICollection<InteriorComponent> Components { get; set; }
    }
}