using System.ComponentModel.DataAnnotations;

namespace Backend_dotnet.Core.DTOs.Auth
{
    public class MeDto
    {
        [Required]
        public string Token {  get; set; }  


    }
}
