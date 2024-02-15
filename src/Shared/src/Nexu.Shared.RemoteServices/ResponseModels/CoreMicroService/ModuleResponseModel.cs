using System;
using System.Collections.Generic;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
   public class ModuleResponseModel
    {
        public Guid? ModuleId { get; set; }
        public Guid? ProjectId { get; set; }
        public List<ListProjectObjectResponseModel> Elements { get; set; }
    }
}
