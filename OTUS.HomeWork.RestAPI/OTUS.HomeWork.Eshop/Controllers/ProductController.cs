using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.DAL;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Services;
using System.Threading.Tasks;

namespace OTUS.HomeWork.Eshop.Controllers
{
    [ApiController]
    [Route("api/product")]
    [Authorize(Policy = "OnlyOwner")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(ProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ProductDTO>> GetProducts(int skip, int limit)
        {
            var products = await _productRepository.GetProductsAsync(skip, limit);
            return _mapper.Map<ProductDTO>(products);
        }
    }
}