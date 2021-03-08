using Microsoft.AspNetCore.Mvc;

namespace OTUS.HomeWork.RestAPI.Controllers
{
    [ApiController]
    [Authorize(Policy = "OnlyOwner")]
    public class OrderController : ControllerBase
    {
        
    }
}