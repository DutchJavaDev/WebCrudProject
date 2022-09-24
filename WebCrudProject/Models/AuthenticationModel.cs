using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Models
{
    [Table("tblAuthenticationModel")]
    public class AuthenticationModel : ISqlModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
