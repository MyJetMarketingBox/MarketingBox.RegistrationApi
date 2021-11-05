using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Affiliate;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models.Lead;
using MarketingBox.RegistrationApi.Models.Lead.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;
using Microsoft.Extensions.Logging;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/register")]
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

            var affResponse = await _affiliateService.IsValidAffiliateAuthInfoAsync(
                MapToGrpc(request.OfferId, affiliateId, apikey));

            if (affResponse.Status == Registration.Service.Grpc.Models.Common.ResultCode.RequiredAuthentication)
            {
                return NotFound(affResponse.Error.Message);
            }

            if (affResponse.Status == Registration.Service.Grpc.Models.Common.ResultCode.Failed)
            {
                return Unauthorized(affResponse.Error.Message);
            }

            var leadResponse = await _registrationService.CreateAsync(
                MapToGrpc(request, affiliateId, apikey));


            return MapToResponse(leadResponse);
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
                    ErrorCode = (int)ErrorType.Unknown
                }
            });
        }
    }
}