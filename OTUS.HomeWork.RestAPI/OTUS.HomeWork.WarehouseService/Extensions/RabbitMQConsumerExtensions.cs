using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.WarehouseService.MessageHandlers;
using OTUS.HomeWork.WarehouseService.Options;
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
                var rabbitMQOption = sp.GetService<IOptions<WarehouseRabbitMQOption>>()?.Value;
                var distributedCache = sp.GetService<IDistributedCache>();
                if (rabbitMQOption == null)
                    throw new ArgumentNullException("RabbitMQ Options is not being initialized");

                return new RabbitMQQueueConsumer(rabbitMQOption.QueueName
                    , new RabbitMqConnectionPool(rabbitMQOption.ConnectionString)
                    , async (body, serializer) =>
                    {
                        using var serviceScope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();
                        List<IMessageHandler> allHandlers = new();
                        allHandlers.Add(new DeliveryResponseMessageHandler(serviceScope, serializer));
                        allHandlers.Add(new UpdateProductCounterByReserveMessageHandler(serviceScope, serializer));
                        allHandlers.Add(new UpdateProductCounterByShipmentMessageHandler(serviceScope, serializer));
                        allHandlers.Add(new UpdateProductCounterByResetReserveMessageHandler(serviceScope, serializer));

                        BrokerMessage message = serializer.DeserializeRequest<BrokerMessage>(body);
                        string cacheKey = message.MessageType + message.Id;
                        if (await distributedCache.GetAsync(cacheKey) != null)
                            return;

                        var handler = allHandlers.FirstOrDefault(g => g.MessageType == message.MessageType);
                        if (handler == null)
                            throw new Exception($"Не найден обработчик сообщения {message.MessageType}");

                        await handler.HandleAsync(body);

                        distributedCache.Set(cacheKey, Array.Empty<byte>(), new DistributedCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.UtcNow.AddDays(2)
                        });
                    });
            });
        }
    }
}