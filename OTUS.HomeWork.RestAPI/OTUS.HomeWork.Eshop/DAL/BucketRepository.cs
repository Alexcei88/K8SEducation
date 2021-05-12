using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.EShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.EShop.DAL
{
    public class BucketRepository
    {
        private readonly OrderContext _orderContext;

        public BucketRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<Bucket[]> UpdateBucketsAsync(IEnumerable<Bucket> buckets, Guid userId)
        {
            foreach (var bucket in buckets)
            {
                var existBucket = await _orderContext.Buckets.FirstOrDefaultAsync(g => g.UserId == bucket.UserId && g.ProductId == bucket.ProductId);
                if (existBucket != null)
                {
                    existBucket.Quantity = bucket.Quantity;
                    _orderContext.Buckets.Update(existBucket);
                }
                else
                    _orderContext.Buckets.Add(bucket);
            }
            await _orderContext.SaveChangesAsync();
            return await _orderContext.Buckets.Where(g => g.UserId == userId).ToArrayAsync();
        }

        public async Task<Bucket[]> GetBucketForUserAsync(Guid userId)
        {
            return await _orderContext.Buckets.Where(g => g.UserId == userId).ToArrayAsync();
        }

        public async Task ClearBucketAsync(Guid userId)
        {
            var buckets = await GetBucketForUserAsync(userId);
            foreach (var bucket in buckets)
                _orderContext.Buckets.Remove(bucket);

            await _orderContext.SaveChangesAsync();
        }
    }
}
