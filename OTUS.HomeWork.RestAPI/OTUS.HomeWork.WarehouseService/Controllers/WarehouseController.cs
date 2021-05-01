using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.WarehouseService.Domain;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController 
        : ControllerBase
    {        
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(ILogger<WarehouseController> logger)
        {
            _logger = logger;
        }

        [HttpPut("/reserve")]
        public Task ReserveProducts(ReserveProductRequestDTO request)
        {
            return null;
        }

        [HttpPut("/shipment")]
        public Task ShipmentProducts(ShipmentRequestDTO request)
        {
            return null;
        }
    }
}
