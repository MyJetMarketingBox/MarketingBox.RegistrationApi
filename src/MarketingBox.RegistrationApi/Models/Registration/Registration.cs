using Destructurama.Attributed;
using JetBrains.Annotations;
using System;

namespace MarketingBox.RegistrationApi.Models.Registration
{
    public class Registration
    {
        #region Personal Info
        public string RegistrationUid { get; set; }
        //public long? RegistrationId { get; set; }
        [CanBeNull] public string CrmStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }

        //[LogMasked(PreserveLength = true)]
        //public string Password { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }
        
        public string Country { get; set; }
        #endregion
        #region Route Info
        // Old name "CampaignId"
        public long? CampaignId { get; set; }
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
        [CanBeNull] public Conversion Conversion { get; set; } = null;
        [CanBeNull] public Brand Brand { get; set; } = null;
    }
}


