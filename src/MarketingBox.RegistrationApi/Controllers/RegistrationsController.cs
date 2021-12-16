using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;
using Microsoft.Extensions.Logging;
using Error = MarketingBox.RegistrationApi.Models.Registration.Contracts.Error;
using ResultCode = MarketingBox.RegistrationApi.Models.Registration.Contracts.ResultCode;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/registrations")]
    public class RegistrationsController : ControllerBase
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAffiliateAuthService _affiliateService;
        private readonly ICustomerService _customerService;

        public RegistrationsController(IRegistrationService registrationService,
            ILogger<RegistrationsController> logger, 
            IAffiliateAuthService affiliateService, 
            ICustomerService customerService)
        {
            _registrationService = registrationService;
            _logger = logger;
            _affiliateService = affiliateService;
            _customerService = customerService;
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(RegistrationCreateRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RegistrationCreateRespone>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [FromBody] RegistrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            var validator = new RegistrationCreateValidations();
            var results = await validator.ValidateAsync(request);
            if (!results.IsValid)
            {
                return BadRequest(results.GetErrors());
            }

            var refistration = await _registrationService.CreateAsync(
                MapToGrpc(request, affiliateId, apikey));

            return MapToResponse(refistration, request);
        }

        [HttpGet]
        [ProducesResponseType(typeof(RegistrationsFullModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RegistrationsFullModel>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [FromQuery] RegistrationSearchRequest request)
        {
            var serviceResponse = await _customerService.GetCustomers(new GetCustomersRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                From = request.FromDate,
                To = request.ToDate,
                Type = request.Type.MapEnum<CustomerType>()
            });

            if (serviceResponse.Error != null)
            {
                if (serviceResponse.Error.Type == ErrorType.Unauthorized)
                    return Unauthorized();

                return BadRequest();
            }
            
            if (serviceResponse.Customers == null || !serviceResponse.Customers.Any())
                return NotFound();

            return Ok(MapToModel(serviceResponse.Customers));
        }
        
        [HttpGet("{uid}")]
        [ProducesResponseType(typeof(RegistrationFullModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RegistrationFullModel>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [Required, FromRoute] string uid)
        {
            var serviceResponse = await _customerService.GetCustomer(new GetCustomerRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                UId = uid
            });
            
            if (serviceResponse.Error != null)
            {
                if (serviceResponse.Error.Type == ErrorType.Unauthorized)
                    return Unauthorized();

                return BadRequest();
            }

            if (serviceResponse.Customer == null)
                return NotFound();

            return Ok(MapToModel(serviceResponse.Customer));
        }

        private static Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateRequest MapToGrpc(
            RegistrationCreateRequest request,
            long affiliateId, string apikey)
        {
            var leadCreateRequest = new Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateRequest()
            {
                GeneralInfo = new ()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Ip = request.Ip,
                    Password = request.Password,
                    Phone = request.Phone,
                    Country = request.Country,
                },
                AdditionalInfo = new ()
                {
                    Funnel = request.Funnel,
                    AffCode = request.AffCode,
                    Sub1 = request.Sub1,
                    Sub2 = request.Sub2,
                    Sub3 = request.Sub3,
                    Sub4 = request.Sub4,
                    Sub5 = request.Sub5,
                    Sub6 = request.Sub6,
                    Sub7 = request.Sub7,
                    Sub8 = request.Sub8,
                    Sub9 = request.Sub9,
                    Sub10 = request.Sub10
                },
                AuthInfo = new AffiliateAuthInfo()
                {
                    AffiliateId = affiliateId,
                    ApiKey = apikey,
                    CampaignId = request.OfferId
                },
                
            };
            return leadCreateRequest;
        }

        private static Registration.Service.Grpc.Models.Affiliate.Contracts.AffiliateAuthRequest MapToGrpc(
            long offerId, long affiliateId, string apikey)
        {
            var affCreateRequest = new Registration.Service.Grpc.Models.Affiliate.Contracts.AffiliateAuthRequest()
            {
                AuthInfo = new AffiliateAuthInfo()
                {
                    AffiliateId = affiliateId,
                    ApiKey = apikey,
                    CampaignId = offerId
                },

            };
            return affCreateRequest;
        }

        private ActionResult <RegistrationCreateRespone> MapToResponse(
            Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateResponse response,
            RegistrationCreateRequest originalRequest
            )
        {
            if (response.Status == Registration.Service.Grpc.Models.Common.ResultCode.RequiredAuthentication)
            {
                return Unauthorized(/*response.Error.Message*/
                    new RegistrationCreateRespone
                    {
                        ResultCode = (int)ResultCode.RequiredAuthentication,
                        Message = EnumExtensions.GetDescription(ResultCode.RequiredAuthentication),
                        Error = new Error()
                        {
                            Message = response.Error.Message,
                            ErrorCode = (int)MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType.InvalidAffiliateInfo
                        }
                    }
                    );
            }

            if (response.Status == Registration.Service.Grpc.Models.Common.ResultCode.Failed)
            {
                return Ok(new RegistrationCreateRespone
                {
                    Registration = new RegistrationModel()
                    {
                        //RegistrationUid = response.UniqueId,
                        Email = originalRequest.Email,
                        FirstName = originalRequest.FirstName,
                        Ip = originalRequest.Ip,
                        LastName = originalRequest.LastName,
                        Phone = originalRequest.Phone,
                        Country = originalRequest.Country,
                        AffCode = originalRequest.AffCode,
                        Funnel = originalRequest.Funnel,
                        OfferId = originalRequest.OfferId,
                        Sub1 = originalRequest.Sub1,    
                        Sub2 = originalRequest.Sub2,
                        Sub3 = originalRequest.Sub3,    
                        Sub4 = originalRequest.Sub4,    
                        Sub5 = originalRequest.Sub5,
                        Sub6 = originalRequest.Sub6,    
                        Sub7 = originalRequest.Sub7,
                        Sub8 = originalRequest.Sub8,
                        Sub9 = originalRequest.Sub9,
                        Sub10 = originalRequest.Sub10
                    },
                    ResultCode = (int)response.Status,
                    Message = EnumExtensions.GetDescription((ResultCode)response.Status),
                    Error = new Error()
                    {
                        Message = response.Error.Message,
                        ErrorCode = (int)response.Error.Type,
                    },
                });
            }

            if (response.Status == Registration.Service.Grpc.Models.Common.ResultCode.CompletedSuccessfully)
            {
                return Ok(new RegistrationCreateRespone
                {
                    Brand = new BrandModel()
                    {
                        //Brand = response.BrandInfo.Brand,
                        CustomerId = response.BrandInfo.Data.CustomerId,
                        LoginUrl = response.BrandInfo.Data.LoginUrl,
                        Token = response.BrandInfo.Data.Token,
                    },
                    Registration = new RegistrationModel()
                    {
                        RegistrationUid = response.RegistrationUId,
                        Email = originalRequest.Email,
                        FirstName = originalRequest.FirstName,
                        Ip = originalRequest.Ip,
                        LastName = originalRequest.LastName,
                        Phone = originalRequest.Phone,
                        Country = originalRequest.Country,
                        AffCode = originalRequest.AffCode,
                        Funnel = originalRequest.Funnel,
                        OfferId = originalRequest.OfferId,
                        Sub1 = originalRequest.Sub1,
                        Sub2 = originalRequest.Sub2,
                        Sub3 = originalRequest.Sub3,
                        Sub4 = originalRequest.Sub4,
                        Sub5 = originalRequest.Sub5,
                        Sub6 = originalRequest.Sub6,
                        Sub7 = originalRequest.Sub7,
                        Sub8 = originalRequest.Sub8,
                        Sub9 = originalRequest.Sub9,
                        Sub10 = originalRequest.Sub10
                    },
                    ResultCode = (int)response.Status,
                    Message = EnumExtensions.GetDescription((ResultCode)response.Status),

                });
            }

            return NotFound(new RegistrationCreateRespone
            {
                Registration = new RegistrationModel()
                {
                    //RegistrationUid = response.UniqueId,
                    Email = originalRequest.Email,
                    FirstName = originalRequest.FirstName,
                    Ip = originalRequest.Ip,
                    LastName = originalRequest.LastName,
                    Phone = originalRequest.Phone,
                    Country = originalRequest.Country,
                    AffCode = originalRequest.AffCode,
                    Funnel = originalRequest.Funnel,
                    OfferId = originalRequest.OfferId,
                    Sub1 = originalRequest.Sub1,
                    Sub2 = originalRequest.Sub2,
                    Sub3 = originalRequest.Sub3,
                    Sub4 = originalRequest.Sub4,
                    Sub5 = originalRequest.Sub5,
                    Sub6 = originalRequest.Sub6,
                    Sub7 = originalRequest.Sub7,
                    Sub8 = originalRequest.Sub8,
                    Sub9 = originalRequest.Sub9,
                    Sub10 = originalRequest.Sub10
                },
                ResultCode = (int)ResultCode.Failed,
                Message = EnumExtensions.GetDescription(ResultCode.Failed),
                Error = new Error()
                {
                    Message = response.Error.Message,
                    ErrorCode = (int)MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType.Unknown
                }
            });
        }
        private RegistrationsFullModel MapToModel(List<CustomerGrpc> registrationReports)
        {
            var registrations = registrationReports.ConvertAll(x => new RegistrationFullModel()
            {
                Conversion = new ConversionModel
                {
                    CrmStatus = x.CrmStatus,
                    Qftd = x.IsDeposit,
                    QftdAt = x.DepositDate
                },
                Brand = new BrandModel()
                {
                    //TODO Add all filds
                    //Token = x.Token
                    //LoginUrl = x.LoginUrl
                    //CustomerId = customer.CustomerId
                },
                Registration = new RegistrationModel()
                {
                    RegistrationUid = x.UId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Country = x.Country,
                    Email = x.Email,
                    Ip = x.Ip,
                    Phone = x.Phone,
                    //TODO Add all filds
                }
            });

            return new RegistrationsFullModel() { Registrations = registrations};
        }
        private RegistrationFullModel MapToModel(CustomerGrpc registrationReport)
        {
            return new RegistrationFullModel()
            {
                Conversion = new ConversionModel
                {
                    CrmStatus = registrationReport.CrmStatus,
                    Qftd = registrationReport.IsDeposit,
                    QftdAt = registrationReport.DepositDate
                },
                Brand = new BrandModel()
                {
                    //TODO Add all filds
                    //Token = x.Token
                    //LoginUrl = x.LoginUrl
                    //CustomerId = customer.CustomerId
                },
                Registration = new RegistrationModel()
                {
                    RegistrationUid = registrationReport.UId,
                    FirstName = registrationReport.FirstName,
                    LastName = registrationReport.LastName,
                    Country = registrationReport.Country,
                    Email = registrationReport.Email,
                    Ip = registrationReport.Ip,
                    Phone = registrationReport.Phone,
                    //TODO Add all filds
                }
            };
        }
    }
}