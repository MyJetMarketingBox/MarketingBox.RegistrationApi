using System.ComponentModel.DataAnnotations;
using Destructurama.Attributed;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationCreateRequest
    {
        #region Personal Info

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 6)]
        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }

        [Required, MinLength(7)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [Required, StringLength(20, MinimumLength = 7)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 7)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }

        [Required]
        [IsEnum]
        public CountryCodeType? CountryCodeType { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 2)]
        public string CountryCode { get; set; }

        #endregion

        #region Route Info

        // Old name "offerId"
        [Required]
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