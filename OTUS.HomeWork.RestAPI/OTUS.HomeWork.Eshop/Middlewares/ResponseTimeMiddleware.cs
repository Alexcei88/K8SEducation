using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OTUS.HomeWork.Eshop.Monitoring;

namespace OTUS.HomeWork.Eshop.Middlewares
{
	/// <summary>
	/// Промежуточный слой для измерения времени выполнения метода
	/// </summary>
	public class ResponseTimeMiddleware
	{
		// Handle to the next Middleware in the pipeline
		private readonly RequestDelegate _next;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="next"></param>
		public ResponseTimeMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Обработчик сообщения
		/// </summary>
		/// <param name="context"></param>
		/// <param name="reporter"></param>
		/// <returns></returns>
		public Task Invoke(HttpContext context, MetricReporter reporter)
		{
			var path = context.Request.Path.Value;
			if (path == "/metrics"
				|| path == "/health")
			{
				// Call the next delegate/middleware in the pipeline
				return _next(context);
			}
			// Start the Timer using Stopwatch
			var watch = new Stopwatch();
			watch.Start();

			context.Response.OnCompleted(() => {
				// Stop the timer and get elapsed time
				watch.Stop();
				long responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
				reporter.RegisterRequest();
				reporter.RegisterResponseTime(context.Response.StatusCode, context.Request.Method + ": " + path, watch.Elapsed);
				// Add the Response time information in the Item variable
				return Task.CompletedTask;
			});
			// Call the next delegate/middleware in the pipeline
			return _next(context);
		}
	}
}
