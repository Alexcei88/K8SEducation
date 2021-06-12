using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using OTUS.HomeWork.EShop.DAL;
using System.Linq;
using OTUS.HomeWork.EShop.Domain;
using AutoMapper;
using OTUS.HomeWork.Clients;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyOwner")]
    public class BucketController : Controller
    {
        private readonly BucketRepository _bucketRepository;
        private readonly PriceServiceClient _priceClient;
        private readonly IMapper _mapper;

        public BucketController(BucketRepository bucketRepository, PriceServiceClient priceClient, IMapper mapper)
        {
            _bucketRepository = bucketRepository;
            _priceClient = priceClient;
            _mapper = mapper;
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<BucketResponseDTO>> UpdateBucket([FromRoute] Guid userId, BucketRequestDTO bucket)
        {
            var buckets = bucket.Items.Select(g => new Bucket
            {
                Quantity = g.Quantity,
                ProductId = g.ProductId,
                UserId = userId
            });
            var updateBuckets = await _bucketRepository.UpdateBucketsAsync(buckets, userId);
            var priceResponse = await _priceClient.PriceAsync(userId, new PriceRequestDTO
            {
                Products = updateBuckets.Select(g => new PProductDTO
                {
                    ProductId = g.ProductId.ToString(),
                    Quantity = g.Quantity
                }).ToList()
            });
            var bucketsDTO = _mapper.Map<BucketResponseDTO>(updateBuckets);
            bucketsDTO.SummaryPrice = priceResponse.SummaryPrice;
            bucketsDTO.Discount = priceResponse.Discount;
            return Ok(bucketsDTO);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<BucketResponseDTO>> GetBucket([FromRoute] Guid userId)
        {
            var buckets = await _bucketRepository.GetBucketForUserAsync(userId);
            var priceResponse = await _priceClient.PriceAsync(userId, new PriceRequestDTO
            {
                Products = buckets.Select(g => new PProductDTO
                {
                    ProductId = g.ProductId.ToString(),
                    Quantity = g.Quantity
                }).ToList()
            });
            var bucketsDTO = _mapper.Map<BucketResponseDTO>(buckets);
            bucketsDTO.SummaryPrice = priceResponse.SummaryPrice;
            bucketsDTO.Discount = priceResponse.Discount;
            return Ok(bucketsDTO);
        }
    }
}
