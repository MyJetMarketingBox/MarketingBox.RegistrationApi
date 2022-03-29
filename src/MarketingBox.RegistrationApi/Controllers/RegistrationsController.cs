using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Requests.Registration;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;
using Microsoft.Extensions.Logging;
using MarketingBox.Sdk.Common.Extensions;
using RegistrationCreateRequest = MarketingBox.Registration.Service.Grpc.Requests.Registration.RegistrationCreateRequest;

namespace MarketingBox.RegistrationApi.Controllers
{
    [ApiController]
    [Route("/api/registrations")]
    public class RegistrationsController : ControllerBase
    {
        private readonly ILogger<RegistrationsController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationsByDateService _customerService;
        private IMapper _mapper;

        public RegistrationsController(IRegistrationService registrationService,
            ILogger<RegistrationsController> logger,
            IRegistrationsByDateService customerService, IMapper mapper)
        {
            _registrationService = registrationService;
            _logger = logger;
            _customerService = customerService;
            this._mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Models.Registration.Registration), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Models.Registration.Registration>> CreateAsync(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apikey,
            [FromBody] Models.Registration.Contracts.RegistrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            var grpcRequest = _mapper.Map<RegistrationCreateRequest>(request);
            grpcRequest.AuthInfo.AffiliateId = affiliateId;
            grpcRequest.AuthInfo.ApiKey = apikey;
            
            var registration = await _registrationService.CreateAsync(grpcRequest);

            return this.ProcessResult(registration, _mapper.Map<Models.Registration.Registration>(registration));
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
            var req = _mapper.Map<RegistrationsGetByDateRequest>(request);
            req.AffiliateId = affiliateId;
            req.ApiKey = apikey;
            var serviceResponse = await _customerService.GetRegistrationsAsync(req);

            return this.ProcessResult(serviceResponse, serviceResponse.Data?
                .Select(_mapper.Map<Models.Registration.Registration>));
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

            return this.ProcessResult(serviceResponse, _mapper.Map<Models.Registration.Registration>(serviceResponse.Data));
        }
    }
}