using Destructurama.Attributed;

namespace MarketingBox.RegistrationApi.Models.Lead
{
    public class LeadModel
    {
        public long LeadId { get; set; }
        public BrandInfo BrandInfo { get; set; }
    }

    public class BrandInfo
    {
        public string CustomerId { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        public string Token { get; set; }

        public string UniqueId { get; set; }

        public string LoginUrl { get; set; }

        public string Broker { get; set; }
    }
}
