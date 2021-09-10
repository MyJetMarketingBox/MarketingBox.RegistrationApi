using MarketingBox.RegistrationApi.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.CampaignBoxes.Requests
{
    public class CampaignBoxesSearchRequest : PaginationRequest<long?>
    {
        [FromQuery(Name = "id")]
        public long? Id { get; set; }
    }
}
