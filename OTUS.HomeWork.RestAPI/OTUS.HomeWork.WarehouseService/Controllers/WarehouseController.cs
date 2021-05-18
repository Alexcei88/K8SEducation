using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.WarehouseService.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OTUS.HomeWork.WarehouseService.Contract.DTO;

namespace OTUS.HomeWork.WarehouseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class WarehouseController 
        : ControllerBase
    {        
        private readonly ILogger<WarehouseController> _logger;
        private readonly Services.WarehouseService _warehouseService;
        private readonly IMapper _mapper;

        public WarehouseController(Services.WarehouseService warehouseService
            , IMapper mapper
            , ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("/reserve")]
        public async Task<ActionResult<ReserveProductResponseDTO>> ReserveProducts(ReserveProductRequestDTO request)
        {
            var products = _mapper.Map<ReserveProduct[]>(request.Products);
            var reserveProducts = await _warehouseService.ReserveProductsAsync(products, request.OrderNumber);

            bool isSuccess = true;
            List<ReserveProductResultDTO> productsResult = new();
            int i = 0;
            foreach (var product in products)
            {
                productsResult.Add(new ReserveProductResultDTO
                {
                    Id = product.ProductId,
                    isSuccess = reserveProducts[i].Count == product.Count,
                    ReserveCount = reserveProducts[i].Count,
                });
                isSuccess &= reserveProducts[i].Count > 0;
            }

            var result = new ReserveProductResponseDTO
            {
                OrderNumber = request.OrderNumber,
                isSuccess = isSuccess,
                Products = productsResult
            };

            return Ok(result);
        }

        [HttpPut("/reserve/cancel")]
        public async Task<ActionResult> ReserveProducts([FromQuery]string orderNumber)
        {
            bool res = await _warehouseService.ResetReserveProductsAsync(orderNumber);
            if (res)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("/shipment")]
        public async Task<ActionResult> ShipmentProducts(ShipmentRequestDTO request)
        {
            bool res = await _warehouseService.ShipmentProductsAsync(request.OrderNumber, request.DeliveryAddress, request.UserId);
            if (res)
                return Ok();
            else
                return BadRequest();
        }
    }
}
