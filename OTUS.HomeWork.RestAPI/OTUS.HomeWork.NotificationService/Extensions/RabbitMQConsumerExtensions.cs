using DataBuffer.BusClient.RabbitMq;
using DataBuffer.BusClient.RabbitMq.Pool;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.NotificationService.DAL;
using System;

namespace OTUS.HomeWork.NotificationService.Extensions
{
    public static class RabbitMQConsumerExtensions
    {
        public static void AddRabbitMQConsumer(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var rabbitMQOption = sp.GetService<IOptions<RabbitMQOption>>()?.Value;
                if (rabbitMQOption == null)
                    throw new ArgumentNullException("RedisMQ Options is not being initialized");

                return new RabbitMQQueueConsumer(rabbitMQOption.ConnectionString
                    , rabbitMQOption.QueueName
                    , new RabbitMqConnectionPool(rabbitMQOption.ConnectionString)
                    , async (body, serializer) =>
                    {
                        using var serviceScope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();                        
                        var repository = serviceScope.ServiceProvider.GetService<NotificationRepository>();

                        NotificationMessage message = serializer.DeserializeRequest<NotificationMessage>(body);
                        body.Position = 0;
                        switch(message.MessageType)
                        {
                            case OrderCreated.TYPE:
                                {
                                    var orderCreated = serializer.DeserializeRequest<OrderCreated>(body);
                                    await repository.AddNotificationAsync(new Domain.Notification
                                    {
                                        UserId = orderCreated.UserId,
                                        CreatedDateUtc = DateTime.UtcNow,
                                        Message = $"Заказ №{orderCreated.OrderNumber} на сумму {orderCreated.Price} успено оплачен"                                        
                                    });
                                break;
                                }
                            case OrderCreatedError.TYPE:
                                {
                                    var orderCreatedError = serializer.DeserializeRequest<OrderCreatedError>(body);
                                    await repository.AddNotificationAsync(new Domain.Notification
                                    {
                                        UserId = orderCreatedError.UserId,
                                        CreatedDateUtc = DateTime.UtcNow,
                                        Message = $"Заказ отменен по причине {orderCreatedError.Message}"
                                    });
                                }
                                break;
                            default:
                                throw new Exception($"Malformed message type {message.MessageType}");
                        }
                    });
            });
        }
    }
}
