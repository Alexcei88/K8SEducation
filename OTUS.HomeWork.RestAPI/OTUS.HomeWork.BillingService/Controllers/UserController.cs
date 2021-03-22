using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.BillingService.Domain;
using OTUS.HomeWork.BillingService.Services;

namespace OTUS.HomeWork.BillingService.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize(Policy = "OnlyOwner")]
    public class UserController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IMapper _mapper;

        public UserController(IBillingService billingService, IMapper mapper)
        {
            _billingService = billingService;
            _mapper = mapper;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<decimal>> CreateUser(Guid userId)
        {
            var balance = await _billingService.CreateBalanceAsync(userId);
            return Ok(new UserDTO
            {
                Balance = balance,
                Id = userId
            });
        }
        
        [HttpGet("{userId}/balance")]
        public async Task<ActionResult<UserDTO>> GetBalance(Guid userId)
        {
            var balance = await _billingService.GetBalanceAsync(userId);
            return Ok(new UserDTO
            {
                Balance = balance,
                Id = userId
            });
        }

        [HttpPost("{userId}/payment")]
        public async Task<ActionResult<PaymentDTO>> MakePayment(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var payment = await _billingService.MakePaymentAsync(paymentRequest);
            return Ok(_mapper.Map<PaymentDTO>(payment));
        }

        [HttpPost("{userId}/payment/rollback")]
        public async Task<ActionResult<PaymentDTO>> RollbackPayment(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var payment = await _billingService.MakePaymentAsync(paymentRequest);
            return Ok(_mapper.Map<PaymentDTO>(payment));
        }

    }
}