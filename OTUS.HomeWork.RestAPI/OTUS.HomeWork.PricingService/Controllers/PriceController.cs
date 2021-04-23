using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.PricingService.Domain;
using System.Threading.Tasks;

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
        public Task<ActionResult<PriceResponseDTO>> GetPrice(PriceRequestDTO request)
        {
            return null;
        }
    }
}
