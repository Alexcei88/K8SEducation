using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.Eshop.DAL;
using OTUS.HomeWork.Eshop.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.EShop.DAL
{
    public class ProductRepository
    {
        private readonly OrderContext _orderContext;

        public ProductRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public Task<Product[]> GetProductsAsync(int skip, int limit)
        {
            return _orderContext.Products.Skip(skip).Take(limit).ToArrayAsync();
        }

        public async Task<Guid> CreateProduct(Product product)
        {
            _orderContext.Products.Add(product);
            await _orderContext.SaveChangesAsync();
            return product.Id;
        }

        public async Task<decimal?> GetPriceOfProducsAsync(Guid productId)
        {
            return (await _orderContext.Products.FirstOrDefaultAsync(g => g.Id == productId))?.Price;
        }
    }
}
