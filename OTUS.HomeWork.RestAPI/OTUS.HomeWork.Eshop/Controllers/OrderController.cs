using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.EShop.Services;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "OnlyOwner")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly DeliveryServiceClient _deliveryServiceClient;
        private readonly IMapper _mapper;

        public OrderController(OrderService orderService
            , DeliveryServiceClient deliveryServiceClient
            , IMapper mapper)
        {
            _orderService = orderService;
            _deliveryServiceClient = deliveryServiceClient;
            _mapper = mapper;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            var newOrder = await _orderService.CreateOrderAsync(userId, orderDTO);
            return Ok(_mapper.Map<OrderDTO>(newOrder));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<OrderDTO[]>> GetOrders([FromRoute] Guid userId, [DefaultValue(0)] int skip, [DefaultValue(20)] int limit)
        {
            var userOrders = await _orderService.GetOrders(userId, skip, limit);
            return Ok(_mapper.Map<OrderDTO[]>(userOrders));
        }

        [HttpPut("{userId}/cancel")]
        public async Task<ActionResult<OrderDTO>> CancelOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            return BadRequest();
        }

        [HttpGet("{userId}/{orderNumber}/location")]
        public async Task<ActionResult<OrderLocationDTO>> GetLocation([FromRoute] Guid userId, [FromRoute]string orderNumber)
        {
            DeliveryLocationDTO location = await _deliveryServiceClient.DeliveryAsync(orderNumber);
            return Ok(_mapper.Map<OrderLocationDTO>(location));
        }


    }
}