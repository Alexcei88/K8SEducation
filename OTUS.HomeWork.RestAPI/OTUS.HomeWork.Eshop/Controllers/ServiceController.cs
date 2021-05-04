using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OTUS.HomeWork.EShop.Controllers
{
	[AllowAnonymous]
	[Route("api/service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
		/// <summary>
		/// Проверка доступности сервисов
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("/health")]
		public ActionResult HealthCheck()
		{
			return Ok();
		}
	}
}
