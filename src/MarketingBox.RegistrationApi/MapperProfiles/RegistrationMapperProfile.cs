using AutoMapper;
using MarketingBox.Registration.Service.Domain.Models.Affiliate;
using MarketingBox.Registration.Service.Domain.Models.Registrations;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.Sdk.Common.Enums;
using GrpcRequests = MarketingBox.Registration.Service.Grpc.Requests;


namespace MarketingBox.RegistrationApi.MapperProfiles
{
    public class RegistrationMapperProfile : Profile
    {
        public RegistrationMapperProfile()
        {
            CreateMap<RegistrationCreateRequest, GrpcRequests.Registration.RegistrationCreateRequest>()
                .ForMember(x => x.AdditionalInfo, x => x.MapFrom(z => z))
                .ForMember(x => x.GeneralInfo, x => x.MapFrom(z => z))
                .ForMember(x => x.AuthInfo, x => x.MapFrom(z => z));
            CreateMap<RegistrationCreateRequest, RegistrationAdditionalInfo>();
            CreateMap<RegistrationCreateRequest, RegistrationGeneralInfo>();
            CreateMap<RegistrationCreateRequest, AffiliateAuthInfo>();

            CreateMap<Registration.Service.Domain.Models.Registrations.Registration,
                    Models.Registration.Registration>()
                .ForMember(x => x.RegistrationUid, x => x.MapFrom(z => z.UniqueId))
                .ForMember(x => x.Brand, x => x.MapFrom(z => z))
                .ForMember(x => x.Conversion, x => x.MapFrom(z => z))
                .ForMember(x => x.CrmStatus, x => x.MapFrom(z => z.CrmStatus.ToCrmStatus()));
            CreateMap<Registration.Service.Domain.Models.Registrations.Registration,
                    Brand>()
                .ForMember(x => x.Token, x => x.MapFrom(z => z.CustomerToken))
                .ForMember(x => x.CustomerId, x => x.MapFrom(z => z.CustomerId))
                .ForMember(x => x.LoginUrl, x => x.MapFrom(z => z.CustomerLoginUrl));

            CreateMap<Registration.Service.Domain.Models.Registrations.Registration, Conversion>()
                .ForMember(x => x.FirstDeposit, x => x.MapFrom(z => z.Status == RegistrationStatus.Approved))
                .ForMember(x => x.FirstDepositDate, x => x.MapFrom(z => z.ConversionDate));
            CreateMap<RegistrationSearchRequest, GrpcRequests.Registration.RegistrationsGetByDateRequest>();
        }
    }
}