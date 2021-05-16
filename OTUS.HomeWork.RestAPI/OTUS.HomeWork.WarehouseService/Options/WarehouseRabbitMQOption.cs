using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.WarehouseService.Options
{
    public class WarehouseRabbitMQOption
        : RabbitMQOption
    {
        public string DeliveryQueueName {get; set;}
    }
}
