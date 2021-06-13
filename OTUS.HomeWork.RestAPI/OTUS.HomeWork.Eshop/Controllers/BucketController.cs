using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.EShop.DAL;
using OTUS.HomeWork.EShop.Domain;
using AutoMapper;
using OTUS.HomeWork.EShop.Services;
using Microsoft.AspNetCore.Authorization;
using OTUS.HomeWork.Clients.Warehouse;
using System.Linq;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyOwner")]
    public class BucketController : Controller
    {
        private readonly BucketRepository _bucketRepository;
        private OrderService _orderService;
        private WarehouseServiceClient _warehouseServiceClient;
        private readonly IMapper _mapper;

        public BucketController(BucketRepository bucketRepository
            , OrderService orderService
            , WarehouseServiceClient warehouseServiceClient
            , IMapper mapper)
        {
            _bucketRepository = bucketRepository;
            _mapper = mapper;
            _orderService = orderService;
            _warehouseServiceClient = warehouseServiceClient;
        }
        
        // TODO нужно сервис по обновлению корзины создать
        [HttpPut("{userId}")]
        public async Task<ActionResult<BucketResponseDTO>> UpdateBucket([FromRoute] Guid userId, BucketRequestDTO bucketRequest)
        {
            var bucket = _mapper.Map<Bucket>(bucketRequest);
            bucket.UserId = userId;

            // проверка наличия заказываемого количества товаров
            _warehouseServiceClient.AddHeader(Constants.USERID_HEADER, userId.ToString());
            var allProducts = await _warehouseServiceClient.ProductInfoAsync(bucket.Items.Select(g => g.ProductId).ToArray());
            foreach(var pr in bucket.Items)
            {
                var product = allProducts.FirstOrDefault(g => g.Id == pr.ProductId);
                if (product == null || pr.Quantity > product.RemainCount)
                    return BadRequest();
            }

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
