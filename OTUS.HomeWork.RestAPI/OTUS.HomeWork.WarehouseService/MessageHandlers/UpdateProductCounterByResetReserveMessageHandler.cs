using Microsoft.Extensions.DependencyInjection;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.WarehouseService.Messages;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.MessageHandlers
{
    public class UpdateProductCounterByResetReserveMessageHandler
        : IMessageHandler
    {
        public string MessageType => UpdateProductCounterByResetReserveRequest.TYPE;

        private readonly IServiceScope _serviceScope;
        private readonly IMessageExchangeSerializer _serializer;

        public UpdateProductCounterByResetReserveMessageHandler(IServiceScope serviceScope, IMessageExchangeSerializer serializer)
        {
            _serviceScope = serviceScope;
            _serializer = serializer;
        }

        public async Task HandleAsync(MemoryStream body)
        {
            var request = _serializer.DeserializeRequest<UpdateProductCounterByResetReserveRequest>(body);
            var warehouseService = _serviceScope.ServiceProvider.GetService<Services.WarehouseService>();
            await warehouseService.UpdateUnReserveCounterAsync(
                request.Products.Select(g => new Domain.ReserveProduct
                {
                    Count = g.Count,
                    ProductId = g.ProductId
                }));
        }
    }
}
