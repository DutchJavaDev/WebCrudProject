using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Services.ORM.Models
{
    // Store its self in the same table that way it can upate its self if we 
    // update it
    [TableClass("tableDefinition")]
    public class TableDefinition : ISqlModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
        public string PropertyArray { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }


        // returns array of (a,b),(a,b)
        //private string[] GetProperties() => PropertyArray.Split(",");

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is TableDefinition definition)
            {
                return Name == definition.Name &&
                       PropertyCount == definition.PropertyCount &&
                       Type == definition.Type
                      // maybe check using diff algo??
                      // leaving this for now
                      ;// && Enumerable.SequenceEqual(GetProperties(), definition.GetProperties());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * PropertyCount;
        }
    }
}
