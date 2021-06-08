using System;
using System.Collections.Generic;
using System.Text;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.DeliveryService.Contract.Messages
{
    public class DeliveryOrderRequest
        : BrokerMessage
    {
        public class DeliveryProduct
        {
            public double Weight { get; set; }

            public double Space { get; set; }

            public Guid ProductId { get; set; }
        }

        public const string TYPE = "DeliveryOrderRequest";

        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime ReadyToShipmentDate { get; set; }

        public List<DeliveryProduct> Products { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public DeliveryOrderRequest()
        { }

        public DeliveryOrderRequest(string id)
        {
            Id = id;
        }

    }
}
