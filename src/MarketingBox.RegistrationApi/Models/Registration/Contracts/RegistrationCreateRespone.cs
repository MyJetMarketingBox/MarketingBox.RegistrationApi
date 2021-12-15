using System.ComponentModel;
using JetBrains.Annotations;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationCreateRespone
    {
        public int ResultCode { get; set; }
        public string Message { get; set; }
        [CanBeNull] public BrandModel Brand { get; set; }
        [CanBeNull] public RegistrationModel Registration { get; set; }
        [CanBeNull] public Error Error { get; set; }
    }

    public enum ResultCode
    {
        [Description("Operation failed")]
        Failed = 0,
        [Description("Operation completed successfully")]
        CompletedSuccessfully = 1,
        [Description("Operation needed authentication")]
        RequiredAuthentication = 2,
    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}

