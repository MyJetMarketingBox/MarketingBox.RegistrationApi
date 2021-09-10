using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.RegistrationApi.Grpc.Models;

namespace MarketingBox.RegistrationApi.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}
