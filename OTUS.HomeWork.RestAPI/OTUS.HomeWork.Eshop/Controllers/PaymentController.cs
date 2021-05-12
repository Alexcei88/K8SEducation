using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.EShop.Services;
using System;
using System.Threading.Tasks;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController
        : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public PaymentController(OrderService orderService
            , IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("{orderId}")]
        public async Task<ActionResult<OrderDTO>> PaymentComplete([FromRoute]Guid orderId, PaymentResultDTO paymentCompletedDTO)
        {
            var order = await _orderService.OrderWasPaid(paymentCompletedDTO, orderId);
            return Ok(_mapper.Map<OrderDTO>(order));
        }
    }
}
