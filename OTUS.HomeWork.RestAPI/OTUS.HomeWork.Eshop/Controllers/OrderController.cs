using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.EShop.Services;
using OTUS.HomeWork.RabbitMq;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Policy = "OnlyOwner")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(OrderService orderService
            , IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            var newOrder = await _orderService.CreateOrderAsync(userId, orderDTO);
            return Ok(_mapper.Map<OrderDTO>(newOrder));
        }

        [HttpPut("{userId}/cancel")]
        public async Task<ActionResult<OrderDTO>> CancelOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            return BadRequest();
        }
    }
}