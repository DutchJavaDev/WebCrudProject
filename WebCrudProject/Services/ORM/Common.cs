using System.Reflection;

namespace WebCrudProject.Services.ORM
{
    public static class Common
    {
        private readonly static Dictionary<Type, string> SQLServerTypes = new()
        {
            {typeof(int), "INT"},
            {typeof(double), "FLOAT"},
            {typeof(string), "NVARCHAR(500)"},
            {typeof(bool), "BIT" },
            {typeof(Guid), "UNIQUEIDENTIFIER"},
            {typeof(float), "FLOAT" },
            {typeof(byte), "TINYINT" },
            {typeof(short), "SMALLINT" },
            {typeof(decimal), "DECIMAL" },
            {typeof(DateTime), "DATETIME"},
        };

        public static IEnumerable<(string, string)> ConverToSQLTypes(IEnumerable<PropertyInfo> properties)
        {
            var list = new HashSet<(string, string)>();

            foreach (var property in properties)
            {
                var name = property.Name;
                var type = GetSQLServerType(property.PropertyType);

                list.Add((name, type));
            }

            return list;
        }

        public static string GetSQLServerType(Type type)
        {
            var _type = SQLServerTypes[type];

            if (string.IsNullOrEmpty(_type))
            {
                return string.Empty;
            }

            return _type;
        }

        public static string EncodeProperties(string name, string type)
        {
            return $"({name}:{type})";
        }
    }
}
