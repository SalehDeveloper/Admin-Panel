using System.ComponentModel.DataAnnotations;

namespace Backend_dotnet.Core.DTOs.Auth
{
    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }    

        public RoleType NewRole {  get; set; }  




    }

    public enum RoleType { 
    ADMIN , 
    MANAGER , 
    USER  

    
    }
}
