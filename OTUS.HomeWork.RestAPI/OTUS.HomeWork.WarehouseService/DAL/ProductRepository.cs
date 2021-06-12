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

        public Task<Product[]> GetProductsAsync(int skip, int limit, string categoryName)
        {
            var products = _warehouseContext.Products as IQueryable<Product>;
            if(!string.IsNullOrEmpty(categoryName))
            {
                products = products.Where(g => g.Category == categoryName);
            }
            return products.OrderBy(s => s.Id).Skip(skip).Take(limit).ToArrayAsync();
        }

        public Task<string[]> GetCategoriesAsync()
        {
            return _warehouseContext.Products.GroupBy(g => g.Category).Select(g => g.Key).ToArrayAsync();
        }

        public async Task<Guid> CreateProduct(Product product)
        {
            _warehouseContext.Products.Add(product);
            await _warehouseContext.SaveChangesAsync();
            return product.Id;
        }

        public Task<ProductCounter[]> GetProductCounterAsync(IEnumerable<Guid> productIds)
        {
            return _warehouseContext.Counters.Where(g => productIds.Contains(g.ProductId)).ToArrayAsync();
        }

        public Task<Product[]> GetProductsAsync(Guid[] productIds)
        {
            return _warehouseContext.Products.Where(g => productIds.Contains(g.Id)).ToArrayAsync();
        }
    }
}
