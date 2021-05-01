using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.DeliveryService.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.DeliveryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly ILogger<DeliveryController> _logger;

        public DeliveryController(ILogger<DeliveryController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public Task<ActionResult<DeliveryResponseDTO>> DeliveryProducts([FromBody]DeliveryRequestDTO deliveryRequestDTO)
        {
            return new Task<ActionResult<DeliveryResponseDTO>>
        }

        [HttpGet("{trackingNumber}")]
        public Task<ActionResult<LocationDTO>> GetLocation([FromRoute] string trackingNumber)
        {
            return null;
        }
    }
}
