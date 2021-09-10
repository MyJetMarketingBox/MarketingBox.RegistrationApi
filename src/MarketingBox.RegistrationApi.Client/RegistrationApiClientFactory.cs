using JetBrains.Annotations;
using MarketingBox.RegistrationApi.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.RegistrationApi.Client
{
    [UsedImplicitly]
    public class RegistrationApiClientFactory: MyGrpcClientFactory
    {
        public RegistrationApiClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
