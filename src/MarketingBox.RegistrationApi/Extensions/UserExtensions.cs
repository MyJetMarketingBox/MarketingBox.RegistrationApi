using System;
using System.Linq;
using System.Security.Claims;

namespace MarketingBox.RegistrationApi.Extensions
{
    public static class UserExtensions
    {
        public static string GetTenantId(this ClaimsPrincipal user)
        {
            return user.GetTenantIdOrDefault() ?? throw new InvalidOperationException("There is no tenant-id claim");
        }

        private static string GetTenantIdOrDefault(this ClaimsPrincipal user)
        {
            return user.GetClaimOrDefault("tenant-id");
        }

        private static string GetClaimOrDefault(this ClaimsPrincipal user, string claim)
        {
            return user.Identities
                .SelectMany(x => x.Claims)
                .Where(c => c.Type == claim)
                .Select(x => x.Value)
                .SingleOrDefault();
        }
    }
}