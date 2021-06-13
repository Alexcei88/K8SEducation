using Microsoft.EntityFrameworkCore;
using OTUS.HomeWork.EShop.Domain;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace OTUS.HomeWork.EShop.DAL
{
    public class BucketRepository
    {
        private readonly OrderContext _orderContext;

        public BucketRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<Bucket> UpdateBucketsAsync(Bucket bucket, Guid userId)
        {
            var existBucket = await _orderContext.Buckets.Include(g => g.Items).FirstOrDefaultAsync(g => g.UserId == userId);
            if (existBucket != null)
            {
                foreach (var it in existBucket.Items)
                {
                    _orderContext.BucketItems.Remove(it);
                }
                existBucket.Items.Clear();               
                _orderContext.Buckets.Update(existBucket);                

                foreach (var it in bucket.Items)
                {
                    it.BucketId = existBucket.Id;
                    existBucket.Items.Add(it);
                }
                _orderContext.Buckets.Update(existBucket);
            }
            else
            {
                foreach (var it in bucket.Items)
                    it.Bucket = bucket;

                _orderContext.Add(bucket);
            }
            await _orderContext.SaveChangesAsync();
            return await _orderContext.Buckets.FirstAsync(g => g.UserId == userId);
        }

        public async Task<Bucket> GetBucketForUserAsync(Guid userId)
        {
            return await _orderContext.Buckets.Include(g => g.Items).FirstOrDefaultAsync(g => g.UserId == userId);
        }

        public async Task ClearBucketAsync(Guid userId)
        {
            var bucket = await GetBucketForUserAsync(userId);
            _orderContext.Buckets.Remove(bucket);
            await _orderContext.SaveChangesAsync();
        }
    }
}
