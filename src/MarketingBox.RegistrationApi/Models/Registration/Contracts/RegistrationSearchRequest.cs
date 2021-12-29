using System;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationSearchRequest
    {
        [FromQuery(Name = "fromDate")]
        public DateTime FromDate { get; set; }

        [FromQuery(Name = "toDate")]
        public DateTime ToDate { get; set; }

        [FromQuery(Name = "type")]
        public SearchFilter Type { get; set; }
    }
}