using System.ComponentModel.DataAnnotations;

namespace WebCrudProject.Auth.Models
{
    public enum UserAuthenticationState : byte
    {
        Login = 0,
        Register = 1,
    }

    public enum ELURoles : byte
    {
        Null,
        Company,
        User,
        Admin,
    }

    public sealed class AuthenticationModel
    {
        public UserAuthenticationState AuthenticationType { get; set; } = UserAuthenticationState.Register;
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
