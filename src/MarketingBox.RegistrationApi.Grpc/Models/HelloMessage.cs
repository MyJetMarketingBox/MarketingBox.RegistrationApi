using System.Runtime.Serialization;
using MarketingBox.RegistrationApi.Domain.Models;

namespace MarketingBox.RegistrationApi.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
