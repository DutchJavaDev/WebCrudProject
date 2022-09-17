using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Services.ORM.Models
{
    // Store its self in the same table that way it can upate its self if we 
    // update it
    [TableClass("tableDefinition")]
    public class TableDefinition : ISqlModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PropertyCount { get; set; }
        public string PropertyArray { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }


        // returns array of (a,b),(a,b)
        public string[] GetProperties() => PropertyArray.Split(",");

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is TableDefinition definition)
            {
                return Name.Equals(definition.Name) &&
                       PropertyCount == definition.PropertyCount;
            }

            return false;
        }

    }
}
