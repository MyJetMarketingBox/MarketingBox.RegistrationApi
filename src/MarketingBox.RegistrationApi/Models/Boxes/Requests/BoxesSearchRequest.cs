using MarketingBox.RegistrationApi.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Boxes.Requests
{
    public class BoxesSearchRequest : PaginationRequest<long?>
    {
        [FromQuery(Name = "id")]
        public long? Id { get; set; }
    }
}
