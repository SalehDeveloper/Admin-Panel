using System.ComponentModel.DataAnnotations;

namespace Backend_dotnet.Core.DTOs.Auth
{
    public class LoginDto
    {

        [Required(ErrorMessage = "UserName is Require")]
        public string UserName { get; set; }
       
        
        [Required(ErrorMessage = "Password is Require")]
        public string Password { get; set; }
    }
}
