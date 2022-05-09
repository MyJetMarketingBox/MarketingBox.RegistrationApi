using System;
using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Models.Registration.Contracts
{
    public class RegistrationSearchRequest
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public SearchFilter? Type { get; set; }
    }
}