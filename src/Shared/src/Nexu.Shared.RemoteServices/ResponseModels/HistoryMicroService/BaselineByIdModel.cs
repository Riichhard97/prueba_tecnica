using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.ResponseModels.HistoryMicroService
{
    public class BaselineByIdModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Active { get; set; }
    }
}