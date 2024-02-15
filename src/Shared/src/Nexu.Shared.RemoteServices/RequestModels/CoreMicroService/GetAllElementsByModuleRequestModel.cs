using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.RequestModels.CoreMicroService
{
    public class GetAllElementsByModuleRequestModel
    {
        public Guid ProjectId { get; set; }
        public Guid? ModuleId { get; set; }
    }
}
