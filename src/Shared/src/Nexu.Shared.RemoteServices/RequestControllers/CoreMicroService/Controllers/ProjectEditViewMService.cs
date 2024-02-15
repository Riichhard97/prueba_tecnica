using Nexu.Shared.RemoteServices.Common;
using Nexu.Shared.RemoteServices.RequestModels;
using Nexu.Shared.RemoteServices.ResponseModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Nexu.Shared.RemoteServices.RequestModels.CoreMicroService;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    //TODO: Change all paths
    public class ProjectEditViewMService : HttpRequestService, IProjectEditViewMService
    {
        private readonly string _serviceName = "Core";
        public ProjectEditViewMService(
            IHttpContextAccessor context,
            IHttpClientFactory httpClient) : base(context, httpClient)
        { }

        public async Task<string> GetElementsByOneGroupId(
            GetElementsByOneGroupIdRequesModel projects)
        {            
            return await PostJson<GetElementsByOneGroupIdRequesModel>(_serviceName,
                "ProjectEditView/GetElementsByOneGroupId",
                projects);
        }

        public async Task<string> GetElementsOtherObjectByGroupId(
         GetElementsByOneGroupIdRequesModel projects)
        {
            return await PostJson<GetElementsByOneGroupIdRequesModel>(_serviceName,
                "ProjectEditView/GetElementsOtherObjectByOneGroupId",
                projects);
        }
       
        public async Task<string> GetAllElementsFormByModule(
            GetAllElementsByModuleRequestModel query)
        {
            return await PostJson<GetAllElementsByModuleRequestModel>(_serviceName,
                "ProjectEditView/GetAllElementsByModule",
                query);
        }
    }
}
