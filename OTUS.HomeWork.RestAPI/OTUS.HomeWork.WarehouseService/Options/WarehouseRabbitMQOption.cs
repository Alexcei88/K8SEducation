using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.WarehouseService.Options
{
    public class WarehouseRabbitMQOption
        : RabbitMQOption
    {
        public string DeliveryRouteKey {get; set; }
        public string EshopNotificationRouteKey { get; set; }
    }
}
