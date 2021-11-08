namespace MarketingBox.RegistrationApi.Models.Registration
{
    public class RegistrationModel
    {
        public long LeadId { get; set; }
        public RegistrationGeneralInfoWithoutPassword RegistrationInfo { get; set; }
        public BrandInfo BrandInfo { get; set; }
    }
}
