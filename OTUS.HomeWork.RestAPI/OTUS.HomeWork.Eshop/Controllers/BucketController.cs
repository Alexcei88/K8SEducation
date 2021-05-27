using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using OTUS.HomeWork.EShop.DAL;
using System.Linq;
using OTUS.HomeWork.EShop.Domain;
using AutoMapper;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyOwner")]
    public class BucketController : Controller
    {
        private readonly BucketRepository _bucketRepository;
        private readonly IMapper _mapper;

        public BucketController(BucketRepository bucketRepository, IMapper mapper)
        {
            _bucketRepository = bucketRepository;
            _mapper = mapper;
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<BucketRequestDTO>> UpdateBucket([FromRoute] Guid userId, BucketRequestDTO bucket)
        {
            var buckets = bucket.Items.Select(g => new Bucket
            {
                Quantity = g.Quantity,
                ProductId = g.ProductId,
                UserId = userId
            });
            var updateBuckets = await _bucketRepository.UpdateBucketsAsync(buckets, userId);
            return Ok(_mapper.Map<BucketRequestDTO>(updateBuckets));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<BucketRequestDTO>> GetBucket([FromRoute] Guid userId)
        {
            var buckets = await _bucketRepository.GetBucketForUserAsync(userId);
            return Ok(_mapper.Map<BucketRequestDTO>(buckets));
        }
    }
}
