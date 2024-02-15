using System;
using Nexu.Shared.Infrastructure;
using Serilog;

namespace Nexu.Shared.AspNetCore
{
    /// <summary>
    /// Sets the account for the current scope and adds the account Id to the <see cref="IDiagnosticContext" /> for logging purposes.
    /// </summary>
    public sealed class DiagnosticContextCurrentCustomerSetter : 
        ICurrentProjectSetter, ICurrentProjectAccessor,
        ICurrentUserSetter, ICurrentUserAccessor,
        ICurrentTokenSetter, ICurrentTokenAccessor
    {
        private readonly CurrentProjectAccessor _currentProjectAccessor;
        private readonly CurrentUserAccessor _currentUserAccessor;
        private readonly CurrentTokenAssesor _currentTokenAccessor;
        private readonly IDiagnosticContext _diagnosticContext;

        public DiagnosticContextCurrentCustomerSetter(
            CurrentProjectAccessor currentProjectAccessor,
            CurrentUserAccessor currentUserAccessor,
            CurrentTokenAssesor currentTokenAccessor,
            IDiagnosticContext diagnosticContext)
        {
            _currentProjectAccessor = currentProjectAccessor;
            _currentUserAccessor = currentUserAccessor;
            _currentTokenAccessor = currentTokenAccessor;
            _diagnosticContext = diagnosticContext;
        }

        Guid ICurrentProjectAccessor.Get()
        {
            return _currentProjectAccessor.Get();
        }

        Guid ICurrentUserAccessor.Get()
        {
            return _currentUserAccessor.Get();
        }

        string ICurrentUserAccessor.GetName()
        {
            return _currentUserAccessor.GetName();
        }

        string ICurrentUserAccessor.GetEmail()
        {
            return _currentUserAccessor.GetEmail();
        }

        void ICurrentProjectSetter.Set(Guid id)
        {
            _currentProjectAccessor.Set(id);
            SetProjectId(id);
        }

        void ICurrentUserSetter.Set(Guid id)
        {
            _currentUserAccessor.Set(id);
            SetUserId(id);
        }

        void ICurrentUserSetter.SetEmail(string email)
        {
            _currentUserAccessor.SetEmail(email);
            SetEmail(email);
        }


        void ICurrentUserSetter.SetName(string name)
        {
            _currentUserAccessor.SetName(name);
            SetName(name);
        }

        Guid? ICurrentProjectAccessor.TryGet()
        {
            return _currentProjectAccessor.TryGet();
        }

        Guid? ICurrentUserAccessor.TryGet()
        {
            return _currentUserAccessor.TryGet();
        }

        bool ICurrentProjectSetter.TrySet(Guid id)
        {
            if (_currentProjectAccessor.TrySet(id))
            {
                SetProjectId(id);
                return true;
            }

            return false;
        }

        bool ICurrentUserSetter.TrySet(Guid id)
        {
            if (_currentUserAccessor.TrySet(id))
            {
                SetUserId(id);
                return true;
            }

            return false;
        }

        private void SetProjectId(Guid id)
        {
            _diagnosticContext.Set("ProjectId", id);
        }

        private void SetUserId(Guid id)
        {
            _diagnosticContext.Set("UserId", id);
        }
        private void SetName(string name)
        {
            _diagnosticContext.Set("Name", name);
        }

        private void SetEmail(string correo)
        {
            _diagnosticContext.Set("Email", correo);
        }

        public void SetToken(string token)
        {
            _currentTokenAccessor.SetToken(token);
        }

        string ICurrentTokenAccessor.GetToken()
        {
            return _currentTokenAccessor.GetToken();            
        }
    }
}
