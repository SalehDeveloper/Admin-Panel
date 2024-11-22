using System.ComponentModel.DataAnnotations;

namespace Backend_dotnet.Core.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "FirstName is Require")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is Require")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "UserName is Require")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Require")]
        public string Email {  get; set; }  


        [Required(ErrorMessage = "Password is Require")]
        public string Password { get; set; }

        public string Address {  get; set; }        
    }
}
