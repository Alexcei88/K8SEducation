using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.DeliveryService.Domain;
using System.Threading.Tasks;
using OTUS.HomeWork.DeliveryService.Contract.DTO;

namespace OTUS.HomeWork.DeliveryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly Services.DeliveryService _deliveryService;
        private readonly IMapper _mapper;

        public DeliveryController(Services.DeliveryService deliveryService
            , IMapper mapper)
        {
            _deliveryService = deliveryService;
            _mapper = mapper;
        }

        //[HttpPost("/calculate")]
        //public ActionResult<DeliveryLocationDTO> TryDeliveryProducts([FromBody] DeliveryRequestDTO deliveryRequestDTO)
        //{
        //    var delivery = _mapper.Map<Delivery>(deliveryRequestDTO);
        //    delivery = _deliveryService.CalculateDelivery(delivery);
        //    if (delivery == null)
        //        return BadRequest(); // означает, что мы не можем доставить по заданному адресу
        //    return Ok(_mapper.Map<DeliveryResponseDTO>(delivery));
        //}

        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<DeliveryLocationDTO>> GetLocation([FromRoute] string orderNumber)
        {
            var deliveryLocation = await _deliveryService.GetLocationOfOrderAsync(orderNumber);
            if (deliveryLocation == null)
                return NotFound();
            return Ok(_mapper.Map<DeliveryLocationDTO>(_mapper.Map<DeliveryLocationDTO>(deliveryLocation)));
        }
    }
}
