using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.WarehouseService.Contract.DTO;
using OTUS.HomeWork.WarehouseService.DAL;

namespace OTUS.HomeWork.WarehouseService.Controllers
{
    [ApiController]
    [Route("api/product")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(ProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ProductDTO[]>> GetProducts([DefaultValue(0)]int skip, [DefaultValue(20)] int limit, [FromQuery]string categoryName)
        {
            // TODO в отдельный сервис вероятно лучше запихнуть и создать какой-то агрегатный класс,соединяющий описание товара и их количество
            var products = await _productRepository.GetProductsAsync(skip, limit, categoryName);
            var counters = await _productRepository.GetProductCounterAsync(products.Select(g => g.Id));
            List<ProductDTO> result = new();
            foreach(var product in products)
            {
                ProductDTO productDTO = _mapper.Map<ProductDTO>(product);
                productDTO.RemainCount = counters.First(g => g.ProductId == product.Id).RemainCount - counters.First(g => g.ProductId == product.Id).ReserveCount;
                if (productDTO.RemainCount < 0)
                    productDTO.RemainCount = 0;
                result.Add(productDTO);
            }
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ProductCategoryDTO[]>> GetCategories()
        {
            var categories = await _productRepository.GetCategoriesAsync();
            return Ok(categories.Select(g => new ProductCategoryDTO
            {
                Name = g
            }));
        }

        [HttpGet("productPrice")]
        public async Task<ActionResult<ProductPriceDTO[]>> GetProductsPrice([FromQuery] Guid[] productIds)
        {
            var products = await _productRepository.GetProductsAsync(productIds);
            return Ok(_mapper.Map<ProductPriceDTO[]>(products));
        }

        [HttpGet("productInfo")]
        public async Task<ActionResult<ProductDTO[]>> GetProducts([FromQuery] Guid[] productIds)
        {
            var products = await _productRepository.GetProductsAsync(productIds);
            var counters = await _productRepository.GetProductCounterAsync(products.Select(g => g.Id));
            List<ProductDTO> result = new();
            foreach (var product in products)
            {
                ProductDTO productDTO = _mapper.Map<ProductDTO>(product);
                productDTO.RemainCount = counters.First(g => g.ProductId == product.Id).RemainCount - counters.First(g => g.ProductId == product.Id).ReserveCount;
                if (productDTO.RemainCount < 0)
                    productDTO.RemainCount = 0;
                result.Add(productDTO);
            }

            return Ok(result);
        }
    }
}