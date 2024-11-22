namespace Backend_dotnet.Core.DTOs.Auth
{
    public class UserInfoResult
    {
        public string Id { get; set; }  

        public string FirstName { get; set; }   

        public string LastName { get; set; }

        public string Email { get; set; }   

        public DateTime CreatedAt { get; set; }

        public IEnumerable<string> Roles { get; set; }  
    }
}
