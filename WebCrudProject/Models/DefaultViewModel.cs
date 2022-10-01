using WebCrudProject.Auth.Models;

namespace WebCrudProject.Models
{
    public class DefaultViewModel
    {
        public AuthenticationModel AuthenticationModel = new();
        
        public string Email { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public bool IsAdmin { get; set; }
    }
}
