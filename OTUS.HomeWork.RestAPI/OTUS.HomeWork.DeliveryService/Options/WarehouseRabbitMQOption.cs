using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.DeliveryService.Options
{
    public class WarehouseRabbitMQOption
        : RabbitMQOption
    {
        public string WarehouseQueueName {get; set;}
    }
}
