using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.RegistrationApi.Models.Lead;
using MarketingBox.RegistrationApi.Models.Lead.Requests;
using MarketingBox.RegistrationApi.Pagination;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/register")]
    public class LeadController : ControllerBase
    {
        private readonly ILeadService _leadService;

        public LeadController(ILeadService leadService)
        {
            _leadService = leadService;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(LeadModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<LeadModel>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")]
            string affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apikey,
            [FromBody] LeadCreateRequest request)
        {

            var response = await _leadService.CreateAsync(
                new Registration.Service.Grpc.Models.Leads.Requests.LeadCreateRequest()
                {
                    GeneralInfo = new Registration.Service.Grpc.Models.Leads.LeadGeneralInfo()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Ip = request.Ip,
                        Password = request.Password,
                        Phone = request.Phone
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
                    Route = await BrandRegisterAsync(request)
                });

            return MapToResponse(response);
        }


        private ActionResult MapToResponse(Registration.Service.Grpc.Models.Leads.Contracts.LeadCreateResponse response)
        {
            if (response.Error != null)
            {
                ModelState.AddModelError("", response.Error.Message);

                return BadRequest(ModelState);
            }

            if (response.BrandInfo == null)
                return NotFound();

            return Ok(new LeadModel
            {
                LeadId = Convert.ToInt64(response.BrandInfo.Data.UniqueId),
                BrandInfo = new BrandInfo()
                {
                    UniqueId = response.BrandInfo.Data.UniqueId,
                    Email = response.BrandInfo.Data.Email,
                    Broker = response.BrandInfo.Data.Broker,
                    CustomerId = response.BrandInfo.Data.CustomerId,
                    LoginUrl = response.BrandInfo.Data.LoginUrl,
                    Token = response.BrandInfo.Data.Token
                }
            });
        }

        private async Task<LeadRouteInfo> BrandRegisterAsync(LeadCreateRequest leadRequest)
        {
            long affiliateId = 1;
            long boxId = 2;
            long campaignId = 3;
            string brand = "Monfex";

            var routeInfo = new LeadRouteInfo()
            {
                AffiliateId = affiliateId,
                BoxId = boxId,
                CampaignId = campaignId,
                Brand = brand
            };
            await Task.Delay(1000);
            return routeInfo;
        }
    }
}