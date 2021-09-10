using Autofac;
using MarketingBox.RegistrationApi.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.RegistrationApi.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAssetsDictionaryClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new RegistrationApiClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
