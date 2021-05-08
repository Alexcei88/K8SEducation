using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.PaymentGatewayService.Domain;
using OTUS.HomeWork.PaymentGatewayService.Services;

namespace OTUS.HomeWork.PaymentGatewayService.Controllers
{
    [ApiController]
    [Route("api/billing")]
    //[Authorize(Policy = "OnlyOwner")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IMapper _mapper;

        public BillingController(IBillingService billingService, IMapper mapper)
        {
            _billingService = billingService;
            _mapper = mapper;
        }
       
        [HttpPost("{userId}/payment")]
        public async Task<ActionResult<PaymentDTO>> MakePayment(Guid userId, PaymentRequestDTO paymentRequest)
        {
            var payment = await _billingService.MakePaymentAsync(userId, paymentRequest);
            return Ok(_mapper.Map<PaymentDTO>(payment));
        }

        [HttpPost("{userId}/refund")]
        public async Task<ActionResult<RefundDTO>> RefundPayment(Guid userId, RefundRequestDTO refundRequest)
        {
            var refund = await _billingService.RefundAsync(userId, refundRequest);
            return Ok(_mapper.Map<RefundDTO>(refund));
        }

    }
}