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
            var existBuckets = await _orderContext.Buckets.Where(g => g.UserId == userId).ToArrayAsync();
            _orderContext.Buckets.RemoveRange(existBuckets);

            _orderContext.Buckets.AddRange(buckets);
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
