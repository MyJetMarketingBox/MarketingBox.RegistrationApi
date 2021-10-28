using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.RegistrationApi.Domain.Extensions;
using MarketingBox.RegistrationApi.Models.Lead;
using MarketingBox.RegistrationApi.Models.Lead.Contracts;
using MarketingBox.RegistrationApi.Models.Validators;
using Microsoft.Extensions.Logging;

using LeadGeneralInfo = MarketingBox.RegistrationApi.Models.Lead.LeadGeneralInfo;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/register")]
    public class LeadController : ControllerBase
    {
        private readonly ILogger<LeadController> _logger;
        private readonly ILeadService _leadService;

        public LeadController(ILeadService leadService,
            ILogger<LeadController> logger)
        {
            _leadService = leadService;
            _logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(LeadCreateRespone), StatusCodes.Status200OK)]
        public async Task<ActionResult<LeadCreateRespone>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromBody] LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            var validator = new LeadCreateValidations();
            var results = await validator.ValidateAsync(request);
            if (!results.IsValid)
            {
                return BadRequest(results.GetErrors());
            }

            var response = await _leadService.CreateAsync(
                MapToGrpc(request, affiliateId, apikey));

            return MapToResponse(response);
        }

        private static Registration.Service.Grpc.Models.Leads.Contracts.LeadCreateRequest MapToGrpc(
            LeadCreateRequest request,
            long affiliateId, string apikey)
        {
            var leadCreateRequest = new Registration.Service.Grpc.Models.Leads.Contracts.LeadCreateRequest()
            {
                GeneralInfo = new Registration.Service.Grpc.Models.Leads.LeadGeneralInfo()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Ip = request.Ip,
                    Password = request.Password,
                    Phone = request.Phone,
                    Country = request.Country,
                },
                AdditionalInfo = new Registration.Service.Grpc.Models.Leads.LeadAdditionalInfo()
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
                AuthInfo = new LeadAuthInfo()
                {
                    AffiliateId = affiliateId,
                    ApiKey = apikey,
                    BoxId = request.OfferId
                },
            };
            return leadCreateRequest;
        }

        private ActionResult <LeadCreateRespone> MapToResponse(Registration.Service.Grpc.Models.Leads.Contracts.LeadCreateResponse response)
        {
            if (response.Status == Registration.Service.Grpc.Models.Common.ResultCode.Failed)
            {
                return Ok(new LeadCreateRespone
                {
                    OriginalData = new LeadGeneralInfo()
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
                return Ok(new LeadCreateRespone
                {
                    LeadId = Convert.ToInt64(response.LeadId),
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

            return NotFound(new LeadCreateRespone
            {
                OriginalData = new LeadGeneralInfo()
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