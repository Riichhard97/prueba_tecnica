using System.Threading.Tasks;
using Nexu.Shared.RemoteServices.RequestControllers;

namespace Nexu.Shared.RemoteServices.EventHandlers
{
    public class UserEventHandler : IUserEventHandler
    {
        private readonly IAuthMicroservice _authMicroservice;

        public UserEventHandler(IAuthMicroservice authMicroservice)
        {
            _authMicroservice = authMicroservice;
        }

        public async Task<bool> IsActive()
        {
            var result = await _authMicroservice.GetUser();
            return result.IsActive;
        }
    }
}
