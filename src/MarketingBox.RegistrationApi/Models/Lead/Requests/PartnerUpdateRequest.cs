namespace MarketingBox.RegistrationApi.Models.Lead.Requests
{
    public class PartnerUpdateRequest
    {
        public LeadGeneralInfo GeneralInfo { get; set; }

        public PartnerInfo Info { get; set; }

        public LeadAdditionalInfo Bank { get; set; }
    }
}