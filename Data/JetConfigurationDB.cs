using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetInteriorApp.Models; 


namespace JetInteriorApp.Data
{
    public class JetConfigurationDB
    {
        [Key]
        public Guid ConfigID { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserID { get; set; }
        public string? Name { get; set; }
        public int SeatingCapacity { get; set; }
        public string? CabinDimensions { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation to Users table
        public UserDB User { get; set; }
        public ICollection<InteriorComponentDB> InteriorComponents { get; set; } = new List<InteriorComponentDB>();
    }
}