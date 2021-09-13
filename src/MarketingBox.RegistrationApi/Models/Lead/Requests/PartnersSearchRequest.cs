using MarketingBox.RegistrationApi.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Lead.Requests
{
    public class PartnersSearchRequest : PaginationRequest<long?>
    {
        [FromQuery(Name = "id")]
        public long? Id { get; set; }
    }
}
