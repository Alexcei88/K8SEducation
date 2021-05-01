using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.NotificationService.DAL;
using OTUS.HomeWork.NotificationService.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OTUS.HomeWork.NotificationService.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationController(NotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications(Guid userId, [DefaultValue(0)] int skip, [DefaultValue(20)] int limit)
        {
            var notifications = await _notificationRepository.GetNotificationAsync(userId, skip, limit);
            return Ok(_mapper.Map<NotificationDTO[]>(notifications));
        }
    }
}
