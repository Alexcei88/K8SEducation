using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.DeliveryService.Contract.Messages;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.WarehouseService.Contract.Messages;

namespace OTUS.HomeWork.DeliveryService.MessageHandlers
{
    public class DeliveryRequestMessageHandler
        : IMessageHandler
    {
        public string MessageType => DeliveryOrderRequest.TYPE;

        private readonly IServiceScope _serviceScope;
        private readonly IMessageExchangeSerializer _serializer;
        private readonly string _warehouseRouteKey;

        public DeliveryRequestMessageHandler(IServiceScope serviceScope, IMessageExchangeSerializer serializer, string warehouseRouteKey)
        {
            _serviceScope = serviceScope;
            _serializer = serializer;
            _warehouseRouteKey = warehouseRouteKey;
        }

        public async Task HandleAsync(MemoryStream body)
        {
            var deliveryService = _serviceScope.ServiceProvider.GetService<Services.DeliveryService>();
            var mqSender = _serviceScope.ServiceProvider.GetService<RabbitMQMessageSender>();
            var request = _serializer.DeserializeRequest<DeliveryOrderRequest>(body);

            var delivery = await deliveryService.CreateDeliveryAsync(request);
            if (delivery == null)
            {
                //throw new System.Exception("Не удалось создать заявк); // означает, что мы не можем доставить продукт по заданному адресу
                await mqSender.SendMessageAsync(new DeliveryOrderResponse
                {
                    IsCanDelivery = false,
                    ErrorDescription = "Нефига не доставлю!!!",
                    OrderNumber = request.OrderNumber,
                }, _warehouseRouteKey);
            }
            else
            {
                await mqSender.SendMessageAsync(new DeliveryOrderResponse
                {
                    IsCanDelivery = true,
                    OrderNumber = request.OrderNumber,
                    ShipmentDate = delivery.Location.ShipmentDate,
                }, _warehouseRouteKey);
            }
        }
    }
}
