namespace JetInteriorApp.Models
{
    public class InteriorComponent
    {
        public Guid ComponentId { get; set; } // same as InteriorComponent.Id
        public string Type { get; set; } // e.g. "Seat", "Screen", etc.
        public string Name { get; set; }

        // Layout geometry (explicit instead of one string "position")
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        // General properties (shared across all types)
        public string Material { get; set; }
        public float Weight { get; set; }
        public string Color { get; set; }
        public string Tier { get; set; }
        public string Accessibility { get; set; }

        // Subtype-specific properties (flexible JSON object)
        public ComponentPropertiesBase Properties { get; set; }
    }
}