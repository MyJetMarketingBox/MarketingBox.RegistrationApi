using Microsoft.Extensions.DependencyInjection;
using NSwag;
using System.Linq;

namespace MarketingBox.RegistrationApi
{
    public static class StartupUtils
    {
        public static void SetupSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerDocument(o =>
            {
                o.Title = "Traffme API";
                o.GenerateEnumMappingDescription = true;

                //o.AddSecurity("affiliate-id", Enumerable.Empty<string>(),
                //    new OpenApiSecurityScheme
                //    {
                //        Type = OpenApiSecuritySchemeType.ApiKey,
                //        Description = "Affiliate Id",
                //        In = OpenApiSecurityApiKeyLocation.Header,
                //        Name = "affiliate-id"
                //    });
                //o.AddSecurity("api-key", Enumerable.Empty<string>(),
                //    new OpenApiSecurityScheme
                //    {
                //        Type = OpenApiSecuritySchemeType.ApiKey,
                //        Description = "Affiliate Api Key",
                //        In = OpenApiSecurityApiKeyLocation.Header,
                //        Name = "api-key"
                //    });
            });
        }
    }
}
