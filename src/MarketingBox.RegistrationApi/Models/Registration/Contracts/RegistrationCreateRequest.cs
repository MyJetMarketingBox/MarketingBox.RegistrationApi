using Destructurama.Attributed;
using MarketingBox.Registration.Service.Domain.Models.Common;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationCreateRequest
    {
        #region Personal Info
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }
        
        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }
        
        public string CountryCode { get; set; }
        public CountryCodeType? CountryCodeType { get; set; }
        
        #endregion
        #region Route Info
        // Old name "offerId"
        public long CampaignId { get; set; }
        // Old name "So"
        public string Funnel { get; set; }
        // Old name "sub"
        public string AffCode { get; set; }
        #endregion

        #region Additional parameters
        public string Sub1 { get; set; }
        public string Sub2 { get; set; }
        public string Sub3 { get; set; }
        public string Sub4 { get; set; }
        public string Sub5 { get; set; }
        public string Sub6 { get; set; }
        public string Sub7 { get; set; }
        public string Sub8 { get; set; }
        public string Sub9 { get; set; }
        public string Sub10 { get; set; }

        #endregion  
    }
}
