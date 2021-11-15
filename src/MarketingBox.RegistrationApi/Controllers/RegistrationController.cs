using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;
using Microsoft.Extensions.Logging;
using MarketingBox.RegistrationApi.Pagination;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/registrations")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAffiliateAuthService _affiliateService;

        public RegistrationController(IRegistrationService registrationService,
            ILogger<RegistrationController> logger, IAffiliateAuthService affiliateService)
        {
            _registrationService = registrationService;
            _logger = logger;
            _affiliateService = affiliateService;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(RegistrationCreateRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RegistrationCreateRespone>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromBody] RegistrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            var validator = new RegistrationCreateValidations();
            var results = await validator.ValidateAsync(request);
            if (!results.IsValid)
            {
                return BadRequest(results.GetErrors());
            }

            var leadResponse = await _registrationService.CreateAsync(
                MapToGrpc(request, affiliateId, apikey));

            return MapToResponse(leadResponse);
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(RegistrationCreateRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Paginated<RegistrationModel, long>>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromBody] ReportSearchRequest request)
        {
            _logger.LogInformation("Get Info {@context}", request);

            if (request.Limit < 1 || request.Limit > 1000)
            {
                ModelState.AddModelError($"{nameof(request.Limit)}", "Should be in the range 1..1000");

                return BadRequest();
            }

            //var response = await _registrationService.CreateAsync(
            //    MapToGrpc(request, affiliateId, apikey));

            //return MapToResponse(response);
            return BadRequest("Not Implemented");
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet("{registrationId}")]
        [ProducesResponseType(typeof(RegistrationCreateRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Paginated<RegistrationModel, long>>> SearchAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [Required, FromRoute] string registrationId)
        {
            _logger.LogInformation("Get Info {@context}", registrationId);

            //var response = await _registrationService.CreateAsync(
            //    MapToGrpc(request, affiliateId, apikey));

            //return MapToResponse(response);
            return BadRequest("Not Implemented");
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
                    So = request.So,
                    Sub = request.Sub,
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

        private ActionResult <RegistrationCreateRespone> MapToResponse(Registration.Service.Grpc.Models.Registrations.Contracts.RegistrationCreateResponse response)
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
                    OriginalData = new RegistrationGeneralInfo()
                    {
                        Email = response.OriginalData?.Email,
                        FirstName = response.OriginalData?.FirstName,
                        Ip = response.OriginalData?.Ip,
                        LastName = response.OriginalData?.LastName,
                        Password = response.OriginalData?.Password,
                        Phone = response.OriginalData?.Phone,
                        Country = response.OriginalData?.Country,
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
                    RegistrationId = Convert.ToInt64(response.RegistrationId),
                    BrandInfo = new BrandInfo()
                    {
                        Brand = response.BrandInfo.Brand,
                        CustomerId = response.BrandInfo.Data.CustomerId,
                        LoginUrl = response.BrandInfo.Data.LoginUrl,
                        Token = response.BrandInfo.Data.Token,
                    },
                    ResultCode = (int)response.Status,
                    Message = EnumExtensions.GetDescription((ResultCode)response.Status),

                });
            }

            return NotFound(new RegistrationCreateRespone
            {
                OriginalData = new RegistrationGeneralInfo()
                {
                    Email = response.OriginalData.Email,
                    FirstName = response.OriginalData.FirstName,
                    Ip = response.OriginalData.Ip,
                    LastName = response.OriginalData.LastName,
                    Password = response.OriginalData.Password,
                    Phone = response.OriginalData.Phone,
                    Country = response.OriginalData.Country
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
    }
}