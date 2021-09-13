using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketingBox.RegistrationApi.Models.Lead.Requests
{
    public class LeadCreateRequest
    {
        public LeadGeneralInfo GeneralInfo { get; set; }

        public PartnerInfo Info { get; set; }

        public LeadAdditionalInfo Bank { get; set; }
    }
}
