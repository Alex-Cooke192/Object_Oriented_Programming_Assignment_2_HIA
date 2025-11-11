using System;
using System.ComponentModel.DataAnnotations;

namespace JIDS.Data
{
    public class ComponentSettings
    {
        [Key]
        public Guid ComponentId { get; set; } // foreign key to InteriorComponent
        public string SettingsJson { get; set; } // stores type-specific JSON
    }
}