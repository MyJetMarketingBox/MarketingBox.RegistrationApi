using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketingBox.RegistrationApi.Models.Lead
{
    public class LeadModel
    {
        public long LeadId { get; set; }

        public LeadGeneralInfo GeneralInfo { get; set; }

        public PartnerInfo Patner { get; set; }

        public LeadAdditionalInfo AdditionalInfo { get; set; }
    }
}
