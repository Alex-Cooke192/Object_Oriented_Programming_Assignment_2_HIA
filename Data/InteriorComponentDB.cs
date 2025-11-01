using System;

namespace JetInteriorApp.Data
{
    public class InteriorComponent
    {
        public Guid ComponentId { get; set; }
        public Guid ConfigId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // e.g. "Seat", "Screen", etc.
        public string Material { get; set; }
        public string Color { get; set; }
        public string Tier { get; set; }
        public string Position { get; set; } // JSON string or "{ x, y }"
        public float Width { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }
        public float Cost { get; set; }

        public ComponentSettings ComponentSettings { get; set; }
    }
}