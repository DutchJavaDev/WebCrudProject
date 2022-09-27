namespace WebCrudProject.Auth.Models
{
    public enum UserAuthenticationState : byte
    {
        Register = 0,
        Login = 1
    }

    public sealed class AuthenticationModel
    {
        public UserAuthenticationState AuthenticationType { get; set; } = UserAuthenticationState.Register;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
