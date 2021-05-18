using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.NotificationService.DAL;
using System;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;

namespace OTUS.HomeWork.NotificationService.Extensions
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
                        var repository = serviceScope.ServiceProvider.GetService<NotificationRepository>();

                        IBrokerMessage message = serializer.DeserializeRequest<IBrokerMessage>(body);
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
                            case OrderWasPayment.TYPE:
                                {
                                    var orderWasPayment = serializer.DeserializeRequest<OrderWasPayment>(body);
                                    await repository.AddNotificationAsync(new Domain.Notification
                                    {
                                        CreatedDateUtc = DateTime.UtcNow,
                                        Message = $"Заказ №{orderWasPayment.OrderNumber} на сумму {orderWasPayment.Price} успешно оплачен",
                                        UserId = orderWasPayment.UserId
                                    });
                                }
                                break;
                            case OrderRefundPayment.TYPE:
                                {
                                    var orderRefundPayment = serializer.DeserializeRequest<OrderRefundPayment>(body);
                                    await repository.AddNotificationAsync(new Domain.Notification
                                    {
                                        CreatedDateUtc = DateTime.UtcNow,
                                        Message = $"Возврат оплаты заказа №{orderRefundPayment.OrderNumber} на сумму {orderRefundPayment.Price}",
                                        UserId = orderRefundPayment.UserId
                                    });
                                }
                                break;
                            case OrderReadyToDelivery.TYPE:
                                {
                                    var orderToDelivery = serializer.DeserializeRequest<OrderReadyToDelivery>(body);
                                    await repository.AddNotificationAsync(new Domain.Notification
                                    {
                                        CreatedDateUtc = DateTime.UtcNow,
                                        Message = $"Заказ ${orderToDelivery.OrderNumber} готов к отгрузке. Вы можете отслеживать заказ на сайте гдемойтовар",
                                        UserId = orderToDelivery.UserId
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
