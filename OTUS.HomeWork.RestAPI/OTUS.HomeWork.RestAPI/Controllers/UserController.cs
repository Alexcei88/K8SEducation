using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.RestAPI.DAL;
using OTUS.HomeWork.RestAPI.Domain;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OTUS.HomeWork.RestAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Random _random = new Random();

        public UserController(UserRepository userRepository, IMapper mapper)		
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
       
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> Get(Guid userId)
        {
            if(_random.Next() % 4 == 0)
                return StatusCode(500);
            var user = await _userRepository.GetUserAsync(userId);
            if (user != null)
                return Ok(_mapper.Map<UserDTO>(user));
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody]UserDTO user)
        {
            var newUser = await _userRepository.CreateUserAsync(_mapper.Map<User>(user));
            return _mapper.Map<UserDTO>(newUser); 
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDTO>> Put(Guid userId, [FromBody] UserDTO user)
        {
            if (userId != user.UserId)
            {
                return BadRequest();
            }
            var updatedUser = await _userRepository.UpdateUserAsync(userId, _mapper.Map<User>(user));
            if (updatedUser == null)
                return NotFound();
            return _mapper.Map<UserDTO>(updatedUser);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(Guid userId)
        {
            int count = await _userRepository.DeleteUserAsync(userId);
            if (count <= 0)
                return NotFound();

            return NoContent();
        }
    }
}
