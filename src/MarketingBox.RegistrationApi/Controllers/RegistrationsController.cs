using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using FluentValidation;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using Microsoft.Extensions.Logging;
using MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Registrations;
using MarketingBox.Registration.Service.Domain.Crm;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using MarketingBox.Sdk.Common.Models.Grpc;
using RegistrationCreateRequest = MarketingBox.RegistrationApi.Models.Registration.Contracts.RegistrationCreateRequest;
using GrpcRegistration = MarketingBox.Registration.Service.Grpc.Models.Registrations.Contracts.Registration;
using ValidationError = MarketingBox.Sdk.Common.Models.ValidationError;
using ValidationException = FluentValidation.ValidationException;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/registrations")]
    public class RegistrationsController : ControllerBase
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationsByDateService _customerService;

        public RegistrationsController(IRegistrationService registrationService,
            ILogger<RegistrationsController> logger,
            IRegistrationsByDateService customerService)
        {
            _registrationService = registrationService;
            _logger = logger;
            _customerService = customerService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Models.Registration.Registration), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Models.Registration.Registration>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromBody] RegistrationCreateRequest request,
            [FromServices] IValidator<RegistrationCreateRequest> validator)
        {
            try
            {
                _logger.LogInformation("Creating new Lead {@context}", request);
                await validator.ValidateAndThrowAsync(request);

                var registration = await _registrationService.CreateAsync(
                    MapToGrpc(request, affiliateId, apikey));

                return this.ProcessResult(registration, MapRegistration(registration, request));
            }
            catch (ValidationException e)
            {
                throw new ApiException(
                    new Error
                    {
                        ErrorMessage = BadRequestException.DefaultErrorMessage,
                        ValidationErrors = e.Errors.Select(x => new ValidationError
                        {
                            ErrorMessage = x.ErrorMessage,
                            ParameterName = x.PropertyName
                        }).ToList()
                    });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Models.Registration.Registration>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Models.Registration.Registration>>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromQuery] RegistrationSearchRequest request)
        {
            var req = new RegistrationsGetByDateRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                From = request.FromDate,
                To = request.ToDate,
                Type = request.Type.MapEnum<RegistrationType>()
            };
            var serviceResponse = await _customerService.GetRegistrationsAsync(req);

            return this.ProcessResult(serviceResponse, MapToModel(serviceResponse.Data?.ToList()));
        }

        [HttpGet("{uid}")]
        [ProducesResponseType(typeof(Models.Registration.Registration), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Models.Registration.Registration>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [Required, FromRoute] string uid)
        {
            var serviceResponse = await _customerService.GetRegistrationAsync(new RegistrationGetRequest()
            {
                AffiliateId = affiliateId,
                ApiKey = apikey,
                RegistrationUId = uid,
            });

            return this.ProcessResult(serviceResponse, MapToModel(serviceResponse.Data));
        }

        private static Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateRequest MapToGrpc(
            RegistrationCreateRequest request,
            long affiliateId, string apikey)
        {
            var leadCreateRequest =
                new Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateRequest()
                {
                    GeneralInfo = new()
                    {
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Ip = request.Ip,
                        Password = request.Password,
                        Phone = request.Phone,
                        CountryCode = request.CountryCode,
                        CountryCodeType = request.CountryCodeType.Value,
                    },
                    AdditionalInfo = new()
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

        private static Models.Registration.Registration MapRegistration(
            Response<GrpcRegistration> response,
            RegistrationCreateRequest originalData)
        {
            if (response.Data==null)
            {
                return null;
            }
            return new Models.Registration.Registration()
            {
                Brand = new Brand()
                {
                    //Brand = response.BrandInfo.Brand,
                    CustomerId = response.Data.BrandInfo.Data.CustomerId,
                    LoginUrl = response.Data.BrandInfo.Data.LoginUrl,
                    Token = response.Data.BrandInfo.Data.Token,
                },
                RegistrationUid = response.Data.RegistrationUId,
                Email = originalData.Email,
                FirstName = originalData.FirstName,
                Ip = originalData.Ip,
                LastName = originalData.LastName,
                Phone = originalData.Phone,
                Country = originalData.CountryCode,
                AffCode = originalData.AffCode,
                Funnel = originalData.Funnel,
                OfferId = originalData.OfferId,
                Sub1 = originalData.Sub1,
                Sub2 = originalData.Sub2,
                Sub3 = originalData.Sub3,
                Sub4 = originalData.Sub4,
                Sub5 = originalData.Sub5,
                Sub6 = originalData.Sub6,
                Sub7 = originalData.Sub7,
                Sub8 = originalData.Sub8,
                Sub9 = originalData.Sub9,
                Sub10 = originalData.Sub10
            };
        }

        private IEnumerable<Models.Registration.Registration> MapToModel(List<RegistrationDetails> registrationReports)
        {
            //Full list
            return registrationReports?.ConvertAll(x => new Models.Registration.Registration()
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
        }

        private Models.Registration.Registration MapToModel(RegistrationDetails registration)
        {
            if (registration is null)
            {
                return null;
            }
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