using Destructurama.Attributed;

namespace MarketingBox.RegistrationApi.Models.Lead
{
    public class RegistrationModel
    {
        public long LeadId { get; set; }
        public RegistrationGeneralInfoWithoutPassword RegistrationInfo { get; set; }
        public BrandInfo BrandInfo { get; set; }
    }
}
