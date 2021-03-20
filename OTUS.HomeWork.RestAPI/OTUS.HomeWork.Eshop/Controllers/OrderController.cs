using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OTUS.HomeWork.Eshop.Controllers
{
    [ApiController]
    [Authorize(Policy = "OnlyOwner")]
    public class OrderController : ControllerBase
    {
        
    }
}