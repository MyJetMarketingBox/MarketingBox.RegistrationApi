using MarketingBox.RegistrationApi.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Campaigns.Requests
{
    public class CampaignsSearchRequest : PaginationRequest<long?>
    {
        [FromQuery(Name = "id")]
        public long? Id { get; set; }
    }
}
