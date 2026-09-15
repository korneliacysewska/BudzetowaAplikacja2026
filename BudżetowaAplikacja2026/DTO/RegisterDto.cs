using System.ComponentModel.DataAnnotations;

namespace BudżetowaAplikacja2026.DTO
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
