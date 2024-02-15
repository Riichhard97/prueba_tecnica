using System;

namespace Nexu.Shared.RemoteServices.ResponseModels.HistoryMicroService
{
    public class BaselineItemModel
    {
        public Guid BaselineId { get; set; }      
        public Guid HistoryId { get; set; }        
        public Guid ProjectId { get; set; }
        public Guid ModuleId { get; set; }
        public Guid GroupId { get; set; }
        public string ObjectState { get; set; } 
    }
}
