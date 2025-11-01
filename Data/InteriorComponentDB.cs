using System;
using System.ComponentModel.DataAnnotations;

namespace JetInteriorApp.Data
{
    public class InteriorComponentDB
    {
        [Key]
        public Guid ComponentID { get; set; }
        public Guid ConfigID { get; set; }
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
        public string PropertiesJson { get; set; } //JSON file containing all the info for that component

        public ComponentSettings InteriorComponentSettings { get; set; }
    }
}