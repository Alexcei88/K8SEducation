using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.PricingService.Domain.DTO;
using OTUS.HomeWork.PricingService.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.PricingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly PriceService _priceService;
        private readonly ILogger<PriceController> _logger;
        private readonly IMapper _mapper;

        public PriceController(PriceService priceService
            , IMapper mapper
            , ILogger<PriceController> logger)
        {
            _priceService = priceService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<CalculatedPriceResponseDTO>> GetPrice([FromRoute]Guid userId, [FromBody]PriceRequestDTO request)
        {
            var result = await _priceService.CalculatePriceAsync(request, userId);
            return Ok(_mapper.Map<CalculatedPriceResponseDTO>(result));
        }
    }
}
