using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;
using Microsoft.Extensions.Logging;
using Error = MarketingBox.RegistrationApi.Models.Registration.Contracts.Error;
using ResultCode = MarketingBox.RegistrationApi.Models.Registration.Contracts.ResultCode;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Crm;
using RegistrationCreateRequest = MarketingBox.RegistrationApi.Models.Registration.Contracts.RegistrationCreateRequest;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/registrations")]
    public class RegistrationsController : ControllerBase
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAffiliateAuthService _affiliateService;
        private readonly IRegistrationsByDateService _customerService;

        public RegistrationsController(IRegistrationService registrationService,
            ILogger<RegistrationsController> logger, 
            IAffiliateAuthService affiliateService,
            IRegistrationsByDateService customerService)
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

            var registration = await _registrationService.CreateAsync(
                MapToGrpc(request, affiliateId, apikey));

            return MapToResponse(registration, request);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Registrations), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Registrations>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [FromQuery] RegistrationSearchRequest request)
        {
            var req = new RegistrationsGetByDateRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                From = request.FromDate,
                To = request.ToDate,
                Type = request.Type
                        .MapEnum<Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationType>()
            };
            var serviceResponse = await _customerService.GetRegistrationsAsync(req);

            if (serviceResponse.Error != null)
            {
                if (serviceResponse.Error.Type == ErrorType.Unauthorized)
                    return Unauthorized();

                return BadRequest();
            }

            return Ok(MapToModel(serviceResponse.Customers));
        }
        
        [HttpGet("{uid}")]
        [ProducesResponseType(typeof(Models.Registration.Registration), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Models.Registration.Registration>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [Required, FromRoute] string uid)
        {
            var serviceResponse = await _customerService.GetRegistrationAsync(new RegistrationGetRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                RegistrationUId = uid,
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
                return base.Ok(new RegistrationCreateRespone
                {
                    Registration = new Models.Registration.Registration()
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
                return base.Ok(new RegistrationCreateRespone
                {
                    Registration = new Models.Registration.Registration()
                    {
                        Brand = new Brand()
                        {
                            //Brand = response.BrandInfo.Brand,
                            CustomerId = response.BrandInfo.Data.CustomerId,
                            LoginUrl = response.BrandInfo.Data.LoginUrl,
                            Token = response.BrandInfo.Data.Token,
                        },
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

            return base.NotFound(new RegistrationCreateRespone
            {
                Registration = new Models.Registration.Registration()
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
        private Registrations MapToModel(List<RegistrationDetails> registrationReports)
        {
            //Empty list
            if (registrationReports == null || !registrationReports.Any())
            {
                return new Registrations() { List = new List<Models.Registration.Registration>() };
            }

            //Full list
            var registrations = registrationReports.ConvertAll(x => new Models.Registration.Registration()
            {
                Conversion = new Conversion
                {
                    FirstDeposit = x.Status == RegistrationStatus.Approved,
                    FirstDepositDate = x.ConversionDate,
                },
                Brand = new Brand()
                {
                    Token = x.CustomerToken,
                    LoginUrl = x.CustomerLoginUrl,
                    CustomerId = x.CustomerId,
                },
                CrmStatus = x.CrmStatus.ToCrmStatus(),
                RegistrationUid = x.RegistrationUid,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Country = x.Country,
                Email = x.Email,
                Ip = x.Ip,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt,
                AffCode = x.AffCode,
                Funnel = x.Funnel,
                Sub1 = x.Sub1,
                Sub2 = x.Sub2,
                Sub3 = x.Sub3,
                Sub4 = x.Sub4,
                Sub5 = x.Sub5,
                Sub6 = x.Sub6,
                Sub7 = x.Sub7,
                Sub8 = x.Sub8,
                Sub9 = x.Sub9,
                Sub10 = x.Sub10,
            });

            return new Registrations() { List = registrations};
        }
        private Models.Registration.Registration MapToModel(RegistrationDetails registration)
        {
            return new Models.Registration.Registration()
            {
                Conversion = new Conversion
                {
                    FirstDeposit = registration.Status == RegistrationStatus.Approved,
                    FirstDepositDate = registration.ConversionDate,
                },
                Brand = new Brand()
                {
                    Token = registration.CustomerToken,
                    LoginUrl = registration.CustomerLoginUrl,
                    CustomerId = registration.CustomerId,
                },
                CrmStatus = registration.CrmStatus.ToCrmStatus(),
                RegistrationUid = registration.RegistrationUid,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Country = registration.Country,
                Email = registration.Email,
                Ip = registration.Ip,
                Phone = registration.Phone,
                CreatedAt = registration.CreatedAt,
                AffCode = registration.AffCode,
                Funnel = registration.Funnel,
                Sub1 = registration.Sub1,
                Sub2 = registration.Sub2,
                Sub3 = registration.Sub3,
                Sub4 = registration.Sub4,
                Sub5 = registration.Sub5,
                Sub6 = registration.Sub6,
                Sub7 = registration.Sub7,
                Sub8 = registration.Sub8,
                Sub9 = registration.Sub9,
                Sub10 = registration.Sub10,
            };
        }
    }
}