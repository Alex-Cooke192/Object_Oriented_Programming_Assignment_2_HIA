using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json; 

namespace JetInteriorApp.Models
{
    public class InteriorComponent
    {
        [Key]
        public Guid ComponentID { get; set; }

        [ForeignKey("JetConfiguration")]
        public Guid ConfigID { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Tier { get; set; }
        public string Material { get; set; }
        public string Position { get; set; }
        public DateTime CreatedAt { get; set; }

        // Flexible JSON for type-specific properties
        public string PropertiesJson { get; set; }

        [NotMapped]
        public ComponentPropertiesBase Properties
        {
            get => string.IsNullOrWhiteSpace(PropertiesJson)
                ? null
                : JsonSerializer.Deserialize<ComponentPropertiesBase>(PropertiesJson);
            set => PropertiesJson = JsonSerializer.Serialize(value);
        }

        public JetConfiguration JetConfiguration { get; set; }
    }
}
