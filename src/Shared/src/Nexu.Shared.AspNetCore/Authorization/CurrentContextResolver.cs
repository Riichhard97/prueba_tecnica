using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Nexu.Shared.Infrastructure;
using Nexu.Shared.Infrastructure.Authorization;

namespace Nexu.Shared.AspNetCore.Authorization
{
    /// <summary>
    /// Obtains the current customer and user IDs from the specified <see cref="ClaimsPrincipal" /> and passes the values
    /// to <see cref="ICurrentProjectSetter" /> and <see cref="ICurrentUserSetter"/>, respectively.
    /// </summary>
    public sealed class CurrentContextResolver
    {
        private readonly ICurrentProjectSetter _currentProjectSetter;
        private readonly ICurrentProjectAccessor _currentProjectAccessor;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ICurrentTokenSetter _tokenSetter;
        private readonly ICurrentTokenAccessor _tokenAccessor;

        public CurrentContextResolver(
            ICurrentProjectSetter currentProjectSetter,
            ICurrentProjectAccessor currentProjectAccessor,
            ICurrentUserSetter currentUserSetter,
            ICurrentUserAccessor currentUserAccessor,
            ICurrentTokenSetter tokenSetter,
            ICurrentTokenAccessor tokenAccessor)        
        {
            _currentProjectSetter = currentProjectSetter;
            _currentProjectAccessor = currentProjectAccessor;
            _currentUserSetter = currentUserSetter;
            _currentUserAccessor = currentUserAccessor;
            _tokenSetter = tokenSetter;
            _tokenAccessor = tokenAccessor;
        }

        private void ResolveProjectId(HttpContext httpContext)
        {
            var id = _currentProjectAccessor.TryGet();
            if (!id.HasValue)
            {
                if (httpContext.Request.RouteValues.TryGetValue("ProjectId", out var value))
                {
                    if (value != null)
                    {
                        if (Guid.TryParse(value.ToString(), out Guid projectId))
                        {
                            _currentProjectSetter.Set(projectId);
                        }
                    }
                }
            }
        }

        private void ResolveEmail(HttpContext httpContext)
        {
            var id = _currentUserAccessor.TryGet();
            if (!id.HasValue)
            {
                var email = httpContext.User.GetEmail();
                if (email != null)
                {
                    _currentUserSetter.SetEmail(email);
                }
            }
        }

        private void ResolveName(HttpContext httpContext)
        {
            var id = _currentUserAccessor.TryGet();
            if (!id.HasValue)
            {
                var name = httpContext.User.GetEmail();
                if (name != null)
                {
                    _currentUserSetter.SetName(name);
                }
            }
        }

        private void ResolveUserId(HttpContext httpContext)
        {
            var id = _currentUserAccessor.TryGet();
            if (!id.HasValue)
            {
                var userId = httpContext.User.GetUserId();
                if (userId.HasValue)
                {
                    _currentUserSetter.Set(userId.Value);
                }
            }
        }

        private void ResolveAttachToken(HttpContext httpContext)
        {
            var id = _currentUserAccessor.TryGet();
            if (!id.HasValue)
            {
                var userId = httpContext.User.GetUserId();
                if (userId.HasValue)
                {
                    httpContext.Request.Headers.TryGetValue("Authorization", out var JwtToken);
                    _tokenSetter.SetToken((string)JwtToken);
                }
            }           
        }

        public void Resolve(HttpContext httpContext)
        {
            ResolveUserId(httpContext);
            ResolveProjectId(httpContext);
            ResolveEmail(httpContext);
            ResolveName(httpContext);
            ResolveAttachToken(httpContext);
        }
    }
}
