//Abstract class for properties of eah type of object - this is polymorphic
namespace JetInteriorApp.Models
{
    public abstract class ComponentPropertiesBase
    {
        public Guid ComponentId { get; set; }
    }
}