using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.WarehouseService.Domain;
using OTUS.HomeWork.WarehouseService.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public IEnumerable<ProductDTO> Get(List<Guid> productIds)
        {
            return null;    
        }

        [HttpPut("/reserve")]
        public Task ReserveProducts(ReserveProductRequestDTO request)
        {
            return null;
        }

        [HttpPut("/shippment")]
        public Task ShipmentProducts(ShipmentRequestDTO request)
        {
            return null;
        }

    }
}
