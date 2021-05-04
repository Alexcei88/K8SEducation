using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.PricingService.Domain;
using OTUS.HomeWork.PricingService.Domain.DTO;

namespace OTUS.HomeWork.PricingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {       
        private readonly ILogger<PriceController> _logger;

        public PriceController(ILogger<PriceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<PriceResponseDTO> GetPrice(PriceRequestDTO request)
        {
            return Ok(new PriceResponseDTO
            {
                SummaryPrice = 10.0m
            });
        }
    }
}
