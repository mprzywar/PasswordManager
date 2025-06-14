using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Core.DTOs
{
    public class PasswordCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Website { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }

    public class PasswordResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}