using System.Threading.Tasks;
using System.Collections.Generic;
using Nexu.Shared.RemoteServices.ResponseModels;
using System;


namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public interface IAuthMicroservice
    {
        Task<UserMicroserviceModel> GetUser();  
        Task<List<UserMicroserviceModel>> GetUserByIds(List<Guid> id);      
        Task<UserMicroserviceModel> GetUserbyId(Guid id);
    }
}
