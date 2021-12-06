using System;
using MarketingBox.Registration.Service.Grpc.Models;
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
        public CustomerType Type { get; set; }
    }
}