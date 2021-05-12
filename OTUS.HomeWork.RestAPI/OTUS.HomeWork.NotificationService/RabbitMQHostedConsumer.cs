using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using OTUS.HomeWork.RabbitMq;

namespace OTUS.HomeWork.NotificationService
{
    public class RabbitMQHostedConsumer
        : IHostedService
        , IDisposable
    {
        private RabbitMQQueueConsumer _consumer;

        public RabbitMQHostedConsumer(RabbitMQQueueConsumer consumer)
        {
            _consumer = consumer;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Stop();
            return Task.CompletedTask;
        }
    }
}
