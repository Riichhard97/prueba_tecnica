using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexu.Shared.RemoteServices.ResponseModels;
using Nexu.Shared.RemoteServices.ResponseModels.HistoryMicroService;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public interface IHistoryMicroservice
    {
        Task Add(AddHistoryLogModel request);
        Task<BaselineByIdModel> GetBaselineById(Guid id);
        Task<List<BaselineItemModel>> GetModulesByBaselineItemId(Guid baselineId, Guid moduleId);
    }
}
