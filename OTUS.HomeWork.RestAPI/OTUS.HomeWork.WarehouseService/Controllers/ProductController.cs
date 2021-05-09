using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Domain.DTO;

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
        public async Task<ActionResult<ProductDTO[]>> GetProducts([DefaultValue(0)]int skip, [DefaultValue(20)] int limit)
        {
            // TODO в отдельный сервис вероятно лучше запихнуть и создать какой-то агрегатный класс,соединяющий описание товара и их количество
            var products = await _productRepository.GetProductsAsync(skip, limit);
            var counters = await _productRepository.GetProductCounter(products.Select(g => g.Id));
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

        //[HttpGet]
        //public IEnumerable<ProductDTO> Get(List<Guid> productIds)
        //{
        //    return null;
        //}

    }
}