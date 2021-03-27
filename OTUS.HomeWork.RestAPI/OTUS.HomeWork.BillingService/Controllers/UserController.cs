using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.BillingService.Domain;
using OTUS.HomeWork.BillingService.Services;

namespace OTUS.HomeWork.BillingService.Controllers
{
    [ApiController]
    [Route("api/user")]
    //[Authorize(Policy = "OnlyOwner")]
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
        public async Task<ActionResult<UserDTO>> CreateUser(Guid userId)
        {
            var balance = await _billingService.CreateBalanceAsync(userId);
            return Ok(new UserDTO
            {
                Balance = balance,
                UserId = userId
            });
        }
        
        [HttpGet("{userId}/balance")]
        public async Task<ActionResult<UserDTO>> GetBalance(Guid userId)
        {
            var balance = await _billingService.GetBalanceAsync(userId);
            return Ok(new UserDTO
            {
                Balance = balance,
                UserId = userId
            });
        }

        [HttpPut("{userId}/balance")]
        public async Task<ActionResult<UserDTO>> AddBalance(Guid userId, BillingTransferRequestDTO transfer)
        {
            var newBalance = await _billingService.AddBalanceAsync(userId, transfer);
            return Ok(new UserDTO
            {
                Balance = newBalance,
                UserId = userId
            });
        }


        [HttpPost("{userId}/payment")]
        public async Task<ActionResult<PaymentDTO>> MakePayment(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var payment = await _billingService.MakePaymentAsync(userId, paymentRequest);
            return Ok(_mapper.Map<PaymentDTO>(payment));
        }

        [HttpPost("{userId}/rollback")]
        public async Task<ActionResult<PaymentDTO>> RollbackPayment(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var payment = await _billingService.RollbackPaymentAsync(userId, paymentRequest);
            return Ok(_mapper.Map<PaymentDTO>(payment));
        }

    }
}