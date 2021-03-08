using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.AuthService.Domain;
using OTUS.HomeWork.RestAPI.Abstraction;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.AuthService.Controllers
{
    [ApiController]
    [Authorize(Policy = "OnlyOwner")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> Get(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user != null)
                return Ok(_mapper.Map<UserDTO>(user));
            else
                return NotFound();
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDTO>> Put(Guid userId, [FromBody] UserDTO user)
        {
            if (userId != user.UserId)
            {
                return BadRequest();
            }
            var updatedUser = await _userService.UpdateUserAsync(userId, _mapper.Map<User>(user));
            if (updatedUser == null)
                return NotFound();
            return _mapper.Map<UserDTO>(updatedUser);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(Guid userId)
        {
            int count = await _userService.DeleteUserAsync(userId);
            if (count <= 0)
                return NotFound();

            return NoContent();
        }
    }
}