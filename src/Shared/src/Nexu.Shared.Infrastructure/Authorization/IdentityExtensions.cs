using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Nexu.Shared.Infrastructure.Authorization
{
    public static class IdentityExtensions
    {
        private static readonly string[] Empty = Array.Empty<string>();

        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetUserId).FirstOrDefault(x => x != default);
        }

        private static Guid? GetUserId(IIdentity identity)
        {
            return GetGuid(identity, JwtClaimTypeNames.UserId);
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetEmail).FirstOrDefault(x => x != default);
        }
        
        public static string GetName(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetName).FirstOrDefault(x => x != default);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetUserName).FirstOrDefault(x => x != default);
        }

        private static string GetEmail(IIdentity identity)
        {
            return GetValue(identity, JwtClaimTypeNames.Email);
        }

        private static string GetName(IIdentity identity)
        {
            return GetValue(identity, JwtClaimTypeNames.Name);
        }

         private static string GetUserName(IIdentity identity)
        {
            return GetValue(identity, JwtClaimTypeNames.DisplayName);
        }

        public static Guid? GetWorkspaceId(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetWorkspaceId).FirstOrDefault(x => x != default);
        }

        private static Guid? GetWorkspaceId(IIdentity identity)
        {
            return GetGuid(identity, JwtClaimTypeNames.WorkspaceId);
        }

        public static Guid? GetJti(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetJti).FirstOrDefault(x => x != default);
        }

        private static Guid? GetJti(IIdentity identity)
        {
            return GetGuid(identity, JwtClaimTypeNames.Jti);
        }

        public static string? GetRole(this ClaimsPrincipal principal)
        {
            return principal.Identities.Select(GetRole).FirstOrDefault(x => x != default);
        }

        private static string? GetRole(IIdentity identity)
        {
            return GetValue(identity, JwtClaimTypeNames.Role);
        }

        public static bool IsSystemUser(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(Roles.System.Admin);
        }

        public static bool IsWorkspaceAdministrator(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(Roles.Workspace.Administrator);
        }

        public static bool IsWorkspaceManagerUser(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(Roles.Workspace.Manager);
        }

        public static bool IsWorkspaceReviewerUser(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(Roles.Workspace.Reviewer);
        }

        public static bool IsWorkspaceEditorUser(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(Roles.Workspace.Editor);
        }

        private static Guid? GetGuid(IIdentity identity, string claimType)
        {
            if (identity is null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var value = GetValue(identity, claimType);
            if (value != null && Guid.TryParse(value, out var parsed))
            {
                return parsed;
            }

            return default;
        }

        private static string GetValue(IIdentity identity, string key)
        {
            return GetValues(identity, key).FirstOrDefault();
        }

        private static IEnumerable<string> GetValues(IIdentity identity, string key)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindAll(key)
                    .Select(x => x.Value);
            }

            return Empty;
        }
    }
}
