using System.Reflection;
using System.Text;

namespace WebCrudProject.Services
{
    public static class InternalExtensions
    {
        public static string[] GetNames(this IEnumerable<PropertyInfo> properties)
            => properties.Where(i => i.Name != "Id").Select(i => i.Name).ToArray();

        public static string ToSingleString(this IEnumerable<string> strings, string seperator = ",")
        {
            var builder = new StringBuilder();
            
            foreach (var str in strings)
            {
                builder.Append($"{str}{(str == strings.Last() ? ";" : $"{seperator} ")}");
            }

            return builder.ToString();
        }
    }
}
