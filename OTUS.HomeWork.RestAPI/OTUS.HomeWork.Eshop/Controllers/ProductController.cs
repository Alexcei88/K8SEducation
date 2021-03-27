using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.DAL;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OTUS.HomeWork.Eshop.Controllers
{
    [ApiController]
    [Route("api/product")]
    //[Authorize(Policy = "OnlyOwner")]
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
            var products = await _productRepository.GetProductsAsync(skip, limit);
            return Ok(_mapper.Map<ProductDTO[]>(products));
        }
    }
}