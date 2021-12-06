using System;
using MarketingBox.RegistrationApi.Pagination;
using MarketingBox.Reporting.Service.Grpc.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class CustomerSearchRequest
    {
        [FromQuery(Name = "fromDate")]
        public DateTime FromDate { get; set; }

        [FromQuery(Name = "toDate")]
        public DateTime ToDate { get; set; }

        [FromQuery(Name = "type")]
        public CustomersReportType Type { get; set; }
    }
}