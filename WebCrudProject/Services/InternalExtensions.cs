using System.Reflection;

namespace WebCrudProject.Services
{
    public static class InternalExtensions
    {
        public static string[] GetNames(this IEnumerable<PropertyInfo> properties)
            => properties.Where(i => i.Name != "Id").Select(i => i.Name).ToArray();
    }
}
