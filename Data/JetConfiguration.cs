using System;
using System.Collections.Generic;

namespace JetInteriorApp.Data
{
    public class JetConfiguration
    {
        public Guid ConfigId { get; set; }
        public Guid UserId { get; set; }
        public string ModelName { get; set; }
        public int SeatingCapacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<InteriorComponent> InteriorComponents { get; set; } = new List<InteriorComponent>();
    }
}