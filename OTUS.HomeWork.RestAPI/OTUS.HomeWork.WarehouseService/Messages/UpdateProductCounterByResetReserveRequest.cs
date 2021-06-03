using OTUS.HomeWork.Common;
using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Messages
{
    public class UpdateProductCounterByResetReserveRequest
        : BrokerMessage
    {
        public class ReserveProduct
        {            
            public long Count { get; set; }

            public Guid ProductId { get; set; }
        }

        public const string TYPE = "UpdateProductCounterRequest";

        public List<ReserveProduct> Products { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; }

        public UpdateProductCounterByResetReserveRequest()
        { }

        public UpdateProductCounterByResetReserveRequest(string id)
        {
            Id = id; 
        }
    }
}
