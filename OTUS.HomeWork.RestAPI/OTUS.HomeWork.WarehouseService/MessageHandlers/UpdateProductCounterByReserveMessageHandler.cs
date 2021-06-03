using Microsoft.Extensions.DependencyInjection;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.WarehouseService.Messages;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.MessageHandlers
{
    public class UpdateProductCounterByReserveMessageHandler
        : IMessageHandler
    {
        public string MessageType => UpdateProductCounterByReserveRequest.TYPE;

        private readonly IServiceScope _serviceScope;
        private readonly IMessageExchangeSerializer _serializer;

        public UpdateProductCounterByReserveMessageHandler(IServiceScope serviceScope, IMessageExchangeSerializer serializer)
        {
            _serviceScope = serviceScope;
            _serializer = serializer;
        }

        public async Task HandleAsync(MemoryStream body)
        {
            var request = _serializer.DeserializeRequest<UpdateProductCounterByReserveRequest>(body);
            var warehouseService = _serviceScope.ServiceProvider.GetService<Services.WarehouseService>();
            await warehouseService.UpdateReserveCounterAsync(
                request.Products.Select(g => new Domain.ReserveProduct
                {
                    Count = g.Count,
                    ProductId = g.ProductId
                }));
        }
    }
}
