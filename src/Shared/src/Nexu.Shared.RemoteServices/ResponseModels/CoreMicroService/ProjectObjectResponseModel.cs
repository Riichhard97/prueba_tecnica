using System;
using System.Collections.Generic;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class ProjectObjectResponseModel
    {
        public string ObjecTypeConfigId { get; set; }
        public string DisplayId { get; set; }
        public string ObjecValuetId { get; set; }
        public string ObjectType { get; set; }
        public string Label { get; set; }
        public string LabelHelp { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string ReviewResult { get; set; }
        public string ReviewStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastReviewedBy { get; set; }
        public string LastReviewedOn { get; set; }
        public int? Order { get; set; }
        public Guid? GroupId { get; set; }
    }
}
