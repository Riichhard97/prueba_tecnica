using Nexu.Shared.RemoteServices.Common;
using Nexu.Shared.RemoteServices.RequestModels;
using Nexu.Shared.RemoteServices.RequestModels.User;
using Nexu.Shared.RemoteServices.ResponseModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using MediatR;


namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public class CoreMicroservice : HttpRequestService, ICoreMicroservice
    {
        public IProjectEditViewMService ProjectEditView {get;}
        private readonly string _serviceName = "Core";
        public CoreMicroservice(
            IHttpContextAccessor context,
            IHttpClientFactory httpClient) : base(context, httpClient)
        {
            ProjectEditView = new ProjectEditViewMService(context ,httpClient);
        }

        //TODO: Refactor to single class responsability
        public async Task<List<GetProjectResponseModel>> GetProjects(GetProjectRequestModel projects)
        {
            return await Post<GetProjectRequestModel , List<GetProjectResponseModel>>(_serviceName,
                "TemplateForm/getProjects",
                projects);
        }

        public async Task<Unit> AddUserProject(ListUserProjectRequestModel userProjectRequest)
        {
            return await Post<ListUserProjectRequestModel, Unit>(_serviceName,
                "UserProject/AddUserProjectList",
                userProjectRequest);
        }
    }
}
