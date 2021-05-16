using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.WarehouseService.Contract.Messages;

namespace OTUS.HomeWork.WarehouseService.MessageHandlers
{
    public class DeliveryResponseMessageHandler
        : IMessageHandler
    {
        public string MessageType => DeliveryOrderResponse.TYPE;

        private readonly IServiceScope _serviceScope;
        private readonly IMessageExchangeSerializer _serializer;

        public DeliveryResponseMessageHandler(IServiceScope serviceScope, IMessageExchangeSerializer serializer)
        {
            _serviceScope = serviceScope;
            _serializer = serializer;
        }

        public async Task HandleAsync(MemoryStream body)
        {
            var warehouseService = _serviceScope.ServiceProvider.GetService<WarehouseService.Services.WarehouseService>();
            var response = _serializer.DeserializeRequest<DeliveryOrderResponse>(body);

            await warehouseService.ConfirmDeliveryOrderAsync(response);
        }
    }
}
