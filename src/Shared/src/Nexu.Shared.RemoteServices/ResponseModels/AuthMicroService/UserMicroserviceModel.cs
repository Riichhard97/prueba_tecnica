using System;

namespace Nexu.Shared.RemoteServices.ResponseModels
{
    public class UserMicroserviceModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public int Status { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public Guid? TwoFactoryTypeID { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
