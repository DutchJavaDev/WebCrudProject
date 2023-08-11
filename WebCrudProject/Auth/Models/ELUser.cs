using Dapper.Contrib.Extensions;
using System.Globalization;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Auth.Models
{
    [Table("tblELUser")]
    public sealed class ELUser : ISqlModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CompressedRoles { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }

        public static IEnumerable<ELURoles> GetRoles(string roles) 
        {
            return roles.Split(",")
                .Select(i => ParseRole(i));
        }

        private static ELURoles ParseRole(string role)
        {
            var parseResult = Enum.TryParse(role, true, out ELURoles result);

            return parseResult ? result : ELURoles.Null;
        }

        public void SetRoles(params ELURoles[] roles)
        {
            CompressedRoles = string.Empty;

            foreach (var role in roles)
            {
                CompressedRoles += $"{role}{(role == roles.Last() ? "" : ",")}";
            }
        }
    }
}
