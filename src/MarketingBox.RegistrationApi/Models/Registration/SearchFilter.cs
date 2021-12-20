using System.ComponentModel;

namespace MarketingBox.RegistrationApi.Models.Registration
{
    public enum SearchFilter
    {
        Registrations = 0,
        [Description("Depositors")]
        QFTDepositors = 1,
        All = 2
    }
}
