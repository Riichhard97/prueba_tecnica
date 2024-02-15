namespace Nexu.Shared.Infrastructure.Authorization
{
    public static class Roles
    {
        public static class System
        {
            public const string Admin = "SystemAdmin";
        }

        public static class Workspace
        {
            public const string Administrator = "Administrator";
            public const string Editor = "Editor";
            public const string Reviewer = "Reviewer";
            public const string Manager = "Manager";
        }
    }
}
