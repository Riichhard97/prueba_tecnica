using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nexu.Shared.RemoteServices.Common;
using Nexu.Shared.RemoteServices.ResponseModels;
using Nexu.Shared.RemoteServices.ResponseModels.HistoryMicroService;

namespace Nexu.Shared.RemoteServices.RequestControllers
{
    public class HistoryMicroservice : HttpRequestService,
        IHistoryMicroservice
    {
        private readonly string _serviceName = "History";
        public HistoryMicroservice(
            IHttpContextAccessor context,
            IHttpClientFactory httpClient) : base(context, httpClient)
        { }

        public async Task Add(AddHistoryLogModel request)
        {
            await Post<AddHistoryLogModel, dynamic>(_serviceName, "api/HistoryLog/Add", request);
        }

        public async Task<BaselineByIdModel> GetBaselineById(Guid id)
        {
            return await Get<BaselineByIdModel>(_serviceName, $"baseline/getbaselinebyid?BaselineId={id}");
        }

        public async Task<List<BaselineItemModel>> GetModulesByBaselineItemId(Guid baselineId, Guid moduleId)
        {
            return await Get<List<BaselineItemModel>> (_serviceName, $"baselineItem/getModulesByBaselineItemId?" +
            $"baselineId={baselineId}&moduleId={moduleId}");
        }       
    }
}
