using Microsoft.Extensions.DependencyInjection;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.WarehouseService.Messages;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.MessageHandlers
{
    public class UpdateProductCounterByShipmentMessageHandler
        : IMessageHandler
    {
        public string MessageType => UpdateProductCounterByShipmentRequest.TYPE;

        private readonly IServiceScope _serviceScope;
        private readonly IMessageExchangeSerializer _serializer;

        public UpdateProductCounterByShipmentMessageHandler(IServiceScope serviceScope, IMessageExchangeSerializer serializer)
        {
            _serviceScope = serviceScope;
            _serializer = serializer;
        }

        public async Task HandleAsync(MemoryStream body)
        {
            var request = _serializer.DeserializeRequest<UpdateProductCounterByShipmentRequest>(body);
            var warehouseService = _serviceScope.ServiceProvider.GetService<Services.WarehouseService>();
            if(!
                await warehouseService.UpdateCounterByShipmentAsync(
                request.Products.Select(g => new Domain.ReserveProduct
                {
                    Count = g.Count,
                    ProductId = g.ProductId
                })))
            {
                throw new System.Exception("Не удалось зарезервировать товар. Должна быть логика по отмену заказа. Кидать сообщение о том, что недостаточно товара");
            }


        }
    }
}
