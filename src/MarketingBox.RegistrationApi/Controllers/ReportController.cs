using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.RegistrationApi.Models.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using MarketingBox.RegistrationApi.Pagination;
using Microsoft.Extensions.Logging;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IRegistrationService _registrationService;

        public ReportController(IRegistrationService registrationService,
            ILogger<ReportController> logger)
        {
            _registrationService = registrationService;
            _logger = logger;
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(Paginated<RegistrationModel, long>), StatusCodes.Status200OK)]
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


    }
}