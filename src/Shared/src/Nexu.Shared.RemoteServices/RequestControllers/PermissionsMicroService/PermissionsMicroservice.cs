using Nexu.Shared.RemoteServices.ResponseModels;
using Nexu.Shared.RemoteServices.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public class PermissionsMicroservice : HttpRequestService, IPermissionsMicroservice
    {
        private readonly string _serviceName = "Permissions";        
        public PermissionsMicroservice(
            IHttpContextAccessor context,
            IHttpClientFactory httpClient) : base(context, httpClient)
        {}

        
        public async Task<List<UserPermissionsModel>> GetUserPermissions()
        {  
            return await Get<List<UserPermissionsModel>>(_serviceName, "permissions/GetUserPermissions");
        }

        public async Task<bool> GetPermissionByOperation(string operationId)
        {
            return await Get<bool>(_serviceName, "permissions/GetPermissionByOperation?OperationId=" + operationId);
        }
    }
}