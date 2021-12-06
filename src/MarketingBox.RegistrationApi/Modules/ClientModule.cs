using Autofac;
using MarketingBox.Registration.Service.Client;

namespace MarketingBox.RegistrationApi.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterRegistrationServiceClient(Program.Settings.RegistrationServiceUrl);
        }
    }
}