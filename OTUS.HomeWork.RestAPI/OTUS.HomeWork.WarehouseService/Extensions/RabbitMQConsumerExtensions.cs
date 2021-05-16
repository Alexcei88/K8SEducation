using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.WarehouseService.MessageHandlers;
using OTUS.HomeWork.WarehouseService.Services;

namespace OTUS.HomeWork.WarehouseService.Extensions
{
    public static class RabbitMQConsumerExtensions
    {
        public static void AddRabbitMQConsumer(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMQHostedConsumer>();
            services.AddSingleton(sp =>
            {
                var rabbitMQOption = sp.GetService<IOptions<RabbitMQOption>>()?.Value;
                if (rabbitMQOption == null)
                    throw new ArgumentNullException("RabbitMQ Options is not being initialized");

                return new RabbitMQQueueConsumer(rabbitMQOption.QueueName
                    , new RabbitMqConnectionPool(rabbitMQOption.ConnectionString)
                    , async (body, serializer) =>
                    {
                        using var serviceScope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();
                        List<IMessageHandler> allHandlers = new();
                        allHandlers.Add(new DeliveryResponseMessageHandler(serviceScope, serializer));

                        IBrokerMessage message = serializer.DeserializeRequest<IBrokerMessage>(body);
                        body.Position = 0;

                        var handler = allHandlers.FirstOrDefault(g => g.MessageType == message.MessageType);
                        if (handler == null)
                            throw new Exception($"Не найден обработчик сообщения {message.MessageType}");

                        await handler.HandleAsync(body);
                    });
            });
        }
    }
}