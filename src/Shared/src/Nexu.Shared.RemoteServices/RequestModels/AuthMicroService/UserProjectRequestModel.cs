using System;

namespace Nexu.Shared.RemoteServices.RequestModels.User
{
    public class UserProjectRequestModel
    {
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
