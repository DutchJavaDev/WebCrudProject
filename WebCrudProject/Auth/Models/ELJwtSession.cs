using Dapper.Contrib.Extensions;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Auth.Models
{
    [Table("tblJwtSession")]
    public class ELJwtSession : ISqlModel
    {
        public int Id { get; set;}
        public int UserId { get; set;}
        public string Email { get; set; } = string.Empty;
        public string CompressedRoles { get; set; } = string.Empty;
        public string SessionId { get; set;} = string.Empty;
        public DateTime ExpireDate { get; set; }
        public DateTime LastUpdated { get; set;}
        public DateTime DateCreated { get; set;}
        public bool IsExpired() => DateTime.Now >= ExpireDate;
    }
}
