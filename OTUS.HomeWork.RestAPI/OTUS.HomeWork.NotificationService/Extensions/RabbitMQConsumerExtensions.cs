using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.NotificationService.Contract.Messages;
using OTUS.HomeWork.NotificationService.DAL;
using System;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.NotificationService.Services;

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
                        
                        var smtpService = serviceScope.ServiceProvider.GetService<SmtpService>();

                        BrokerMessage message = serializer.DeserializeRequest<BrokerMessage>(body);
                        body.Position = 0;
                        switch(message.MessageType)
                        {
                            case OrderCreated.TYPE:
                                {
                                    var orderCreated = serializer.DeserializeRequest<OrderCreated>(body);
                                    smtpService.SendEmail("Заказ оплачен", @$"Заказ №{ orderCreated.OrderNumber}
                                        на сумму { orderCreated.Price}
                                        успешно оплачен", orderCreated.UserEmail);
                                break;
                                }
                            case OrderCreatedError.TYPE:
                                {
                                    var orderCreatedError = serializer.DeserializeRequest<OrderCreatedError>(body);
                                    smtpService.SendEmail("Заказ отменен", $"Заказ отменен по причине {orderCreatedError.Message}", orderCreatedError.UserEmail);
                                }
                                break;
                            case OrderWasPayment.TYPE:
                                {
                                    var orderWasPayment = serializer.DeserializeRequest<OrderWasPayment>(body);
                                    smtpService.SendEmail("Заказ оплачен", $"Заказ №{orderWasPayment.OrderNumber} на сумму {orderWasPayment.Price} успешно оплачен", orderWasPayment.UserEmail);
                                }
                                break;
                            case OrderRefundPayment.TYPE:
                                {
                                    var orderRefundPayment = serializer.DeserializeRequest<OrderRefundPayment>(body);
                                    smtpService.SendEmail("Возврат денежных средства", $"Возврат оплаты заказа №{ orderRefundPayment.OrderNumber} на сумму { orderRefundPayment.Price}"
                                        , orderRefundPayment.UserEmail);
                                }
                                break;
                            case OrderReadyToDelivery.TYPE:
                                {
                                    var orderToDelivery = serializer.DeserializeRequest<OrderReadyToDelivery>(body);
                                    smtpService.SendEmail("Товар отгружен", $"Заказ ${orderToDelivery.OrderNumber} отгружен. Отслеживайте местоположения заказа на сайте http://гдемойтовар/{orderToDelivery.OrderNumber}"
                                        , orderToDelivery.UserEmail);
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
