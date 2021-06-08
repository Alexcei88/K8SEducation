using OTUS.HomeWork.Common;
using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Messages
{
    public class UpdateProductCounterByShipmentRequest
        : BrokerMessage
    {
        public class ReserveProduct
        {            
            public long Count { get; set; }

            public Guid ProductId { get; set; }
        }

        public const string TYPE = "UpdateProductCounterByShipmentRequest";

        public List<ReserveProduct> Products { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public UpdateProductCounterByShipmentRequest()
        { }

        public UpdateProductCounterByShipmentRequest(string id)
        {
            Id = id; 
        }
    }
}
