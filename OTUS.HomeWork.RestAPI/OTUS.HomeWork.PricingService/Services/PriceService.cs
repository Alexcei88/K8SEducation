using OTUS.HomeWork.Clients.Warehouse;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.PricingService.Domain;
using OTUS.HomeWork.PricingService.Domain.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.PricingService.Services
{
    public class PriceService
    {
        private readonly WarehouseServiceClient _warehouseServiceClient;
        
        public PriceService(WarehouseServiceClient warehouseServiceClient)
        {
            _warehouseServiceClient = warehouseServiceClient;
        }

        public async Task<PriceResult> CalculatePriceAsync(PriceRequestDTO request, Guid userId)
        {
            var products = request.Products.Where(g => g.Quantity > 0).ToArray();
            if (!products.Any())
                return new PriceResult
                {
                    Discount = 0,
                    SummaryPrice = 0,
                    Products = new System.Collections.Generic.List<PriceResult.CalculatedProductPrice>()
                };

            _warehouseServiceClient.AddHeader(Constants.USERID_HEADER, userId.ToString());
            var basePrices = await _warehouseServiceClient.ProductPriceAsync(products.Select(g => Guid.Parse(g.ProductId)).ToArray());
            var discount = (100 - new Random().Next(0, 30))/100.0m;
            PriceResult result = new();
            foreach(var prod in products)
            {
                var price = basePrices.First(k => k.Id == Guid.Parse(prod.ProductId)).BasePrice;
                result.SummaryPrice += price * prod.Quantity;
            }
            result.Discount = discount;
            result.SummaryPrice *= discount;
            result.SummaryPrice = decimal.Round(result.SummaryPrice, 2);
            return result;
        }
    }
}
