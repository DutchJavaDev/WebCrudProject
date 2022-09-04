using System.ComponentModel.DataAnnotations;

namespace WebCrudProject.Models
{
    public class AuthenticationModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReferenceId { get; set; }

        public string ReturnUrl { get; set; }
    }
}
