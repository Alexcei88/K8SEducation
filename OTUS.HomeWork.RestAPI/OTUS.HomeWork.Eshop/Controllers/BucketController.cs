using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.EShop.DAL;
using OTUS.HomeWork.EShop.Domain;
using AutoMapper;
using OTUS.HomeWork.EShop.Services;
using Microsoft.AspNetCore.Authorization;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyOwner")]
    public class BucketController : Controller
    {
        private readonly BucketRepository _bucketRepository;
        private OrderService _orderService;
        private readonly IMapper _mapper;

        public BucketController(BucketRepository bucketRepository
            , OrderService orderService
            , IMapper mapper)
        {
            _bucketRepository = bucketRepository;
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<BucketResponseDTO>> UpdateBucket([FromRoute] Guid userId, BucketRequestDTO bucketRequest)
        {
            var bucket = _mapper.Map<Bucket>(bucketRequest);
            bucket.UserId = userId;

            var updateBucket = await _bucketRepository.UpdateBucketsAsync(bucket, userId);
            (decimal totalPrice, decimal discount) price = await _orderService.CalculateTotalPriceAsync(updateBucket, userId, true);
            var bucketsDTO = _mapper.Map<BucketResponseDTO>(updateBucket);
            bucketsDTO.SummaryPrice = price.totalPrice;
            bucketsDTO.Discount = price.discount;
            return Ok(bucketsDTO);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<BucketResponseDTO>> GetBucket([FromRoute] Guid userId)
        {
            var bucket = await _bucketRepository.GetBucketForUserAsync(userId);
            if (bucket == null)
                return NotFound();

            (decimal totalPrice, decimal discount) price = await _orderService.CalculateTotalPriceAsync(bucket, userId);
            var bucketsDTO = _mapper.Map<BucketResponseDTO>(bucket);
            bucketsDTO.SummaryPrice = price.totalPrice;
            bucketsDTO.Discount = price.discount;
            return Ok(bucketsDTO);
        }
    }
}
