using Microsoft.AspNetCore.Mvc;

namespace MarketingBox.RegistrationApi.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static string GetTenantId(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetTenantId();
        }
    }
}
