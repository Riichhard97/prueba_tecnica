using System;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class AddHistoryLogModel
    {
        public Guid ProjectId { get; set; }
        public Guid ModuleId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ObjectState { get; set; }
    }
}
