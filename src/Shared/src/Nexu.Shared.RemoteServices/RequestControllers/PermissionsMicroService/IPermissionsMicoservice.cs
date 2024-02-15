using Nexu.Shared.RemoteServices.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public interface IPermissionsMicroservice
    {
        Task<List<UserPermissionsModel>> GetUserPermissions();  
        Task<bool> GetPermissionByOperation(string operationId);
    }
}