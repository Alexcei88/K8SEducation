using OTUS.HomeWork.Common;
using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Messages
{
    public class UpdateProductCounterByReserveRequest
        : BrokerMessage
    {
        public class ReserveProduct
        {            
            public long Count { get; set; }

            public Guid ProductId { get; set; }
        }

        public const string TYPE = "UpdateProductCounterByReserveRequest";

        public List<ReserveProduct> Products { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; }

        public UpdateProductCounterByReserveRequest()
        { }

        public UpdateProductCounterByReserveRequest(string id)
        {
            Id = id;
        }

    }
}
