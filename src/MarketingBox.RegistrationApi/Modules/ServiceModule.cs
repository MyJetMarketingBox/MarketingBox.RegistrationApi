using Autofac;
using FluentValidation;
using MarketingBox.Registration.Service.Client;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;

namespace MarketingBox.RegistrationApi.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationCreateValidations>()
                .As<IValidator<RegistrationCreateRequest>>()
                .SingleInstance();
        }
    }
}
