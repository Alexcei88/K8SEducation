using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.WarehouseService.Contract.Messages
{
    public class DeliveryOrderResponse
        : BrokerMessage
    {
        public const string TYPE = "DeliveryOrderResponse";

        public bool IsCanDelivery { get; set; }

        public string ErrorDescription { get; set; }
        public DateTime ShipmentDate { get; set; }        

        public string OrderNumber { get; set; }

        public override string MessageType => TYPE;

        public override  string Id { get; }

        public DeliveryOrderResponse()
        { }

        public DeliveryOrderResponse(string id)
        {
            Id = id;
        }

    }
}
