using AutoMapper;
using DataBuffer.BusClient.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Services;
using OTUS.HomeWork.NotificationService.Contract.Messages;
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
        private readonly RabbitMQMessageSender _mqSender;

        public OrderController(OrderService orderService
            , IMapper mapper
            , RabbitMQMessageSender mqSender)
        {
            _orderService = orderService;
            _mapper = mapper;
            _mqSender = mqSender;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<CreatedOrderDTO>> CreateOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            try
            {
                Order order = _mapper.Map<Order>(orderDTO);
                order.UserId = userId;
                var newOrder = await _orderService.CreateOrderAsync(order);
                return Ok(_mapper.Map<CreatedOrderDTO>(newOrder));
            }
            catch(Exception ex)
            {
                await _mqSender.SendMessageAsync(new OrderCreatedError
                {
                    UserId = userId,
                    Message = "Not enough balance to complete payment",
                });

                return BadRequest("Not enough balance to complete payment");
            }
        }

        [HttpPut("{userId}/cancel")]
        public async Task<ActionResult<CreatedOrderDTO>> CancelOrder([FromRoute] Guid userId, CreateOrderDTO orderDTO)
        {
            try
            {
                Order order = _mapper.Map<Order>(orderDTO);
                order.UserId = userId;
                var newOrder = await _orderService.CreateOrderAsync(order);
                // send notification
                await _mqSender.SendMessageAsync(new OrderCreated
                {
                    UserId = userId,
                    BillingAddressId = newOrder.BillingId.ToString(),
                    OrderNumber = newOrder.OrderNumber.ToString(),
                    Price = newOrder.TotalPrice,
                }); ;
                return Ok(_mapper.Map<CreatedOrderDTO>(newOrder));
            }
            catch (Exception ex)
            {
                await _mqSender.SendMessageAsync(new OrderCreatedError
                {
                    UserId = userId,
                    Message = "Not enough balance to complete payment",
                });

                return BadRequest("Not enough balance to complete payment");
            }
        }
    }
}