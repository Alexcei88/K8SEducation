using Microsoft.AspNetCore.Mvc;

namespace OTUS.HomeWork.RestAPI.Controllers
{
    [Route("api/[controller]")]
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
