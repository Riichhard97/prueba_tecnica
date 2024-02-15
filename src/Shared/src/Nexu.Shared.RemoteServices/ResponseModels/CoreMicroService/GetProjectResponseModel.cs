using System;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class GetProjectResponseModel
    {
        public Guid Id { get; set; }
        public Guid? TemplateReferenceId { get; set; }
        public Guid? ChangeRequestFormId { get; set; }
        public Guid? ChangeRequestFormWorkFlowId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Name { get; set; }
        public bool isDefaultTemplate { get; set; }
        public bool isTemplate { get; set; }
        public int Status { get; set; }
        public string WorkflowName { get; set; }
    }
}
