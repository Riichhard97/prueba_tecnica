using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nexu.Shared.RemoteServices.RequestModels;
using Nexu.Shared.RemoteServices.RequestModels.User;
using Nexu.Shared.RemoteServices.ResponseModels;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public interface ICoreMicroservice
    {    
        IProjectEditViewMService ProjectEditView {get;}

        //TODO: Refactor to single class responsability
        Task<List<GetProjectResponseModel>> GetProjects(GetProjectRequestModel projects);
        Task<Unit> AddUserProject(ListUserProjectRequestModel projects);

    }
}
