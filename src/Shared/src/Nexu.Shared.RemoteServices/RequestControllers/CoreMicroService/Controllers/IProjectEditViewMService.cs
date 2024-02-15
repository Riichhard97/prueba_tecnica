using Nexu.Shared.RemoteServices.RequestModels;
using Nexu.Shared.RemoteServices.RequestModels.CoreMicroService;
using Nexu.Shared.RemoteServices.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public interface IProjectEditViewMService
    {
        Task<string> GetElementsByOneGroupId(GetElementsByOneGroupIdRequesModel projects);
        Task<string> GetElementsOtherObjectByGroupId(GetElementsByOneGroupIdRequesModel projects);
        Task<string> GetAllElementsFormByModule(GetAllElementsByModuleRequestModel query);
    }
}
