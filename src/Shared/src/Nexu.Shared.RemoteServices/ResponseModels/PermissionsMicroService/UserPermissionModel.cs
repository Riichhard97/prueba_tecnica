using System;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class UserPermissionsModel
    {
        public Guid UserId { get; set; }

        public Guid ModuleId { get; set; }

        public Guid OperationSettingId { get; set; }

        public string ModuleName { get; set; }

        public string OperationSettingName { get; set; }

        public bool Allowed { get; set; }

    }
}