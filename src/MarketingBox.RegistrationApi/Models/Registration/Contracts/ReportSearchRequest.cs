using System;
using MarketingBox.RegistrationApi.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class ReportSearchRequest : PaginationRequest<long?>
    {
        [FromQuery(Name = "fromDate")]
        public DateTime FromDate { get; set; }

        [FromQuery(Name = "toDate")]
        public DateTime ToDate { get; set; }

        [FromQuery(Name = "type")]
        public int Type { get; set; }
    }
}