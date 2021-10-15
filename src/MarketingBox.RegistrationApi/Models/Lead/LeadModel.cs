using Destructurama.Attributed;

namespace MarketingBox.RegistrationApi.Models.Lead
{
    public class LeadModel
    {
        public long LeadId { get; set; }
        public LeadGeneralInfoWithoutPassword LeadInfo { get; set; }
        public BrandInfo BrandInfo { get; set; }
    }
}
