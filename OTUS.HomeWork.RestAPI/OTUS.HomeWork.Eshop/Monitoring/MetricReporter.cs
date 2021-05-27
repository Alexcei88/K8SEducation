using System;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace OTUS.HomeWork.EShop.Monitoring
{
    public class MetricReporter
    {
        private readonly ILogger<MetricReporter> _logger;
        private readonly Counter _requestCounter;
        private readonly Histogram _responseTimeHistogram;
        private readonly Counter _createOrderCounter;
        private readonly Counter _failedOrderCounter;

        public MetricReporter(ILogger<MetricReporter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _requestCounter =
                Metrics.CreateCounter("total_requests", "The total number of requests serviced by this API.");

            _createOrderCounter =
                Metrics.CreateCounter("created_order", "The total number of created order by eshop service");

            _failedOrderCounter =
                Metrics.CreateCounter("failed_order", "The total number of failed order by eshop service");

            _responseTimeHistogram = Metrics.CreateHistogram("request_duration_seconds",
                "The duration in seconds between the response to a request.", new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                    LabelNames = new[] { "status_code", "method" }
                });
        }

        public void RegisterRequest()
        {
            _requestCounter.Inc();
        }

        public void RegisterResponseTime(int statusCode, string method, TimeSpan elapsed)
        {
            _responseTimeHistogram.Labels(statusCode.ToString(), method).Observe(elapsed.TotalSeconds);
        }

        public void RegisterCreateOrder()
        {
            _createOrderCounter.Inc();
        }

        public void RegisterFailedOrder()
        {
            _failedOrderCounter.Inc();
        }
    }
}