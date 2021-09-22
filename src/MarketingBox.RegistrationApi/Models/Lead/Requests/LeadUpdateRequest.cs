namespace MarketingBox.RegistrationApi.Models.Lead.Requests
{
    public class LeadUpdateRequest
    {
        public LeadGeneralInfo GeneralInfo { get; set; }

        public AffiliateInfo Info { get; set; }
    }
}