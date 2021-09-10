using MarketingBox.RegistrationApi.Models.Partners;

namespace MarketingBox.RegistrationApi.Models.Campaigns
{
    public class Revenue
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}