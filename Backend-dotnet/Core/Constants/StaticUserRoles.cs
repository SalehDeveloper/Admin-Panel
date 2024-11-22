namespace Backend_dotnet.Core.Constants
{
    //This class will be used to avoid typing errors 
    public static class StaticUserRoles
    {
        public const string OWNER = "OWNER";

        public const string ADMIN = "ADMIN";

        public const string MANAGER = "MANAGER";

        public const string USER = "USER";

        public const string OwnerAdmin = "ADMIN, OWNER";

        public const string OwnerAdminManager = "OWNER, ADMIN, MANAGER";

        public const string OwnerAdminManagerUser = "OWNER, ADMI, MANAGER, USER";


    }
}
