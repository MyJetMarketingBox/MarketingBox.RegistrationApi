using System.Collections.Generic;
using MarketingBox.Registration.Service.Grpc.Models;

namespace MarketingBox.RegistrationApi.Models
{
    public class CustomerModel
    {
        public List<CustomerGrpc> Customers { get; set; }
    }
}