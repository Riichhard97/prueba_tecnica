using System;
using System.Collections.Generic;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class ListProjectObjectResponseModel
    {
        public string displayId { get; set; }
        public List<ProjectObjectResponseModel> ProjectObjects { get; set; }
        public List<string> Covers { get; set; }
    }
}
