using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Services;
using System;
using System.Threading.Tasks;

namespace OTUS.HomeWork.Eshop.Controllers
{
    [ApiController]
    [Route("api/order")]
   // [Authorize(Policy = "OnlyOwner")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(OrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<CreatedOrderDTO>> CreateOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            Order order = _mapper.Map<Order>(orderDTO);
            order.UserId = userId;
            var newOrder = await _orderService.CreateOrderAsync(order);
            // send notification
            //if()

            return Ok(_mapper.Map<CreatedOrderDTO>(newOrder));
        }
    }
}