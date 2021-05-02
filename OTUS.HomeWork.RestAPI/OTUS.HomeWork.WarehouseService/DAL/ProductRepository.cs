using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.WarehouseService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.WarehouseService.DAL
{
    public class ProductRepository
    {
        private readonly WarehouseContext _warehouseContext;

        public ProductRepository(WarehouseContext orderContext)
        {
            _warehouseContext = orderContext;
        }

        public Task<Product[]> GetProductsAsync(int skip, int limit)
        {
            return _warehouseContext.Products.OrderBy(s => s.Id).Skip(skip).Take(limit).ToArrayAsync();
        }

        public async Task<Guid> CreateProduct(Product product)
        {
            _warehouseContext.Products.Add(product);
            await _warehouseContext.SaveChangesAsync();
            return product.Id;
        }

        public Task<ProductCounter[]> GetProductCounter(IEnumerable<Guid> productIds)
        {
            return _warehouseContext.Counters.Where(g => productIds.Contains(g.ProductId)).ToArrayAsync();
        }

        public async Task<decimal?> GetPriceOfProductAsync(Guid productId)
        {
            return (await _warehouseContext.Products.FirstOrDefaultAsync(g => g.Id == productId))?.BasePrice;
        }
    }
}
