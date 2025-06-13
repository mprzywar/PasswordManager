using Microsoft.AspNetCore.Mvc;
using PasswordManager.API.Services;
using PasswordManager.Core.DTOs;
using PasswordManager.Core.Interfaces;
using PasswordManager.Core.Models;
using BCrypt.Net;

namespace PasswordManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _userRepository.UserExistsAsync(dto.Email))
            {
                return BadRequest("Użytkownik z takim emailem już istnieje");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _userRepository.CreateUserAsync(user);

            var token = _jwtService.GenerateToken(user.Id, user.Email);

            return Ok(new { Token = token, Message = "Rejestracja zakończona sukcesem" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Nieprawidłowy email lub hasło");
            }

            var token = _jwtService.GenerateToken(user.Id, user.Email);

            return Ok(new { Token = token, Message = "Logowanie zakończone sukcesem" });
        }
    }
}