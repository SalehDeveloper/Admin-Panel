namespace Backend_dotnet.Helpers
{
    public class JwtHelper
    {
        public string key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public double DuartionInDays { get; set; }
    }
}
