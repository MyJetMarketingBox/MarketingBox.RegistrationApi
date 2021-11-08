using System.ComponentModel;
using JetBrains.Annotations;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationCreateRespone
    {
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public long? RegistrationId { get; set; }
        [CanBeNull] public BrandInfo BrandInfo { get; set; }
        [CanBeNull] public RegistrationGeneralInfo OriginalData { get; set; }
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
    //public enum ErrorType
    //{
    //    [Description("Unknown")]
    //    Unknown = 0,
    //    [Description("Invalid Parameter")]
    //    InvalidParameter = 1,
    //    [Description("Already Exist")]
    //    AlreadyExist = 2,
    //    [Description("Invalid Email")]
    //    InvalidEmail = 3,
    //    [Description("Invalid User Name Or Password")]
    //    InvalidUserNameOrPassword = 4,
    //    [Description("Invalid Country")]
    //    InvalidCountry = 5,
    //    [Description("Invalid Personal Data")]
    //    InvalidPersonalData = 6
    //}
    public class Error
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}

