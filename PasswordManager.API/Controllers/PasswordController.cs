using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.API.Services;
using PasswordManager.Core.DTOs;
using PasswordManager.Core.Interfaces;
using PasswordManager.Core.Models;
using System.Security.Claims;

namespace PasswordManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PasswordsController : ControllerBase
    {
        private readonly IPasswordRepository _passwordRepository;
        private readonly EncryptionService _encryptionService;

        public PasswordsController(IPasswordRepository passwordRepository, EncryptionService encryptionService)
        {
            _passwordRepository = passwordRepository;
            _encryptionService = encryptionService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswords()
        {
            var userId = GetUserId();
            var passwords = await _passwordRepository.GetPasswordsByUserIdAsync(userId);

            var response = passwords.Select(p => new PasswordResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Website = p.Website,
                Username = p.Username,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPassword(int id)
        {
            var userId = GetUserId();
            var password = await _passwordRepository.GetPasswordByIdAsync(id, userId);

            if (password == null)
            {
                return NotFound();
            }

            var decryptedPassword = _encryptionService.Decrypt(password.EncryptedPassword);

            return Ok(new
            {
                Id = password.Id,
                Title = password.Title,
                Description = password.Description,
                Website = password.Website,
                Username = password.Username,
                Password = decryptedPassword,
                CreatedAt = password.CreatedAt,
                UpdatedAt = password.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword(PasswordCreateDto dto)
        {
            var userId = GetUserId();
            var encryptedPassword = _encryptionService.Encrypt(dto.Password);

            var password = new Password
            {
                Title = dto.Title,
                Description = dto.Description,
                EncryptedPassword = encryptedPassword,
                Website = dto.Website,
                Username = dto.Username,
                UserId = userId
            };

            await _passwordRepository.CreatePasswordAsync(password);

            var response = new PasswordResponseDto
            {
                Id = password.Id,
                Title = password.Title,
                Description = password.Description,
                Website = password.Website,
                Username = password.Username,
                CreatedAt = password.CreatedAt,
                UpdatedAt = password.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPassword), new { id = password.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id)
        {
            var userId = GetUserId();
            var result = await _passwordRepository.DeletePasswordAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}