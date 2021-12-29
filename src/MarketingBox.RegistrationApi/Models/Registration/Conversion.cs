using System;

namespace MarketingBox.RegistrationApi.Models.Registration
{
    public class Conversion
    {
        public bool FirstDeposit { get; set; } = false;
        public DateTime? FirstDepositDate { get; set; } = null;
    }
}