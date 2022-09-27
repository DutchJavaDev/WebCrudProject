using Dapper.Contrib.Extensions;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Auth.Models
{
    [Table("tblELUser")]
    public class ELUser : ISqlModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CompressedRoles { get; set; } = string.Empty;
        public int ELReferenceId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
