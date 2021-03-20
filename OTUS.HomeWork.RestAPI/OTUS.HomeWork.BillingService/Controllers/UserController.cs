using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.BillingService.Domain;
using OTUS.HomeWork.BillingService.Services;

namespace OTUS.HomeWork.BillingService.Controllers
{
    [ApiController]
    [Authorize(Policy = "OnlyOwner")]
    public class UserController : ControllerBase
    {
        private readonly IBillingService _billingService;

        public UserController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<decimal>> CreateUser(Guid userId)
        {
            var balance = await _billingService.CreateBalance(userId);
            return Ok(new UserDto
            {
                Balance = balance,
                Id = userId
            });
        }
        
        [HttpGet("{userId}/balance")]
        public async Task<ActionResult<UserDto>> GetBalance(Guid userId)
        {
            var balance = await _billingService.GetBalance(userId);
            return Ok(new UserDto
            {
                Balance = balance,
                Id = userId
            });
        }

        [HttpPut("{userId}/balance")]
        public async Task<ActionResult<decimal>> ChangeBalance(Guid userId, [FromQuery]decimal balance)
        {
            var modifiedBalance = await _billingService.ChangeBalance(userId, balance);
            return Ok(new UserDto
            {
                Balance = modifiedBalance,
                Id = userId
            });
        }
    }
}