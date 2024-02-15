using Nexu.Shared.RemoteServices.ResponseModels;
using Nexu.Shared.RemoteServices.RequestModels.AuthMicroService;
using Nexu.Shared.RemoteServices.Common;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System;


namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public class AuthMicroservice : HttpRequestService, IAuthMicroservice
    {
        private readonly string _serviceName = "Auth";        
        public AuthMicroservice(
            IHttpContextAccessor context,
            IHttpClientFactory httpClient) : base(context, httpClient)
        {}

        public async Task<UserMicroserviceModel> GetUser()
        {  
            return await Get<UserMicroserviceModel>(_serviceName, "User/GetInfo");
        }

        public async Task<List<UserMicroserviceModel>> GetUserByIds(List<Guid> id)
        {  
            return await Post<GetUsersByIdRequestModel, 
            List<UserMicroserviceModel>>(_serviceName, "User/GetUsersByList", 
             new GetUsersByIdRequestModel { Id = id });
        }

         public async Task<UserMicroserviceModel> GetUserbyId(Guid id)
        {  
            return await Get<UserMicroserviceModel>(_serviceName, "User/id?Id=" + id);
        }
    }
}
