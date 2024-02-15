namespace Nexu.Shared.AspNetCore
{
    public static class AuthConstants
    {
        public const string SystemAdminPolicy = "SystemAdmin";
        public const string WorkspaceAdminOrUserPolicy = "WorkspaceAdministratorOrUser";
        public const string WorkspaceUserPolicy = "WorkspaceUser";
        public const string WorkspaceAdminPolicy = "WorkspaceAdministrator";
        public const string WorkspaceEditorPolicy = "WorkspaceEditor";
        public const string WorkspaceManagerPolicy = "WorkspaceManager";
        public const string WorkspaceReviewerPolicy = "WorkspaceReviewer";
    }
}
