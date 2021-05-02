using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.DeliveryService.Domain;
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

        private readonly Services.DeliveryService _deliveryService;

        private readonly IMapper _mapper;

        public DeliveryController(Services.DeliveryService deliveryService
            , IMapper mapper
            , ILogger<DeliveryController> logger)
        {
            _deliveryService = deliveryService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryResponseDTO>> DeliveryProducts([FromBody]DeliveryRequestDTO deliveryRequestDTO)
        {
            var delivery = _mapper.Map<Delivery>(deliveryRequestDTO);
            delivery = await _deliveryService.CreateDeliveryAsync(delivery);
            if (delivery == null)
                return BadRequest(); // означает, что мы не можем доставить продукт в эту точку
            return Ok(_mapper.Map<DeliveryResponseDTO>(delivery));
        }

        [HttpPost("/calculate")]
        public ActionResult<DeliveryLocationDTO> TryDeliveryProducts([FromBody] DeliveryRequestDTO deliveryRequestDTO)
        {
            var delivery = _mapper.Map<Delivery>(deliveryRequestDTO);
            delivery = _deliveryService.CalculateDelivery(delivery);
            if (delivery == null)
                return BadRequest(); // означает, что мы не можем доставить продукт в эту точку
            return Ok(_mapper.Map<DeliveryResponseDTO>(delivery));
        }

        [HttpGet("/{trackingNumber}")]
        public async Task<ActionResult<DeliveryLocationDTO>> GetLocation([FromRoute] string trackingNumber)
        {
            var deliveryLocation = await _deliveryService.GetLocationOfOrderAsync(trackingNumber);
            return Ok(_mapper.Map<DeliveryLocationDTO>(_mapper.Map<DeliveryLocationDTO>(deliveryLocation)));
        }
    }
}
