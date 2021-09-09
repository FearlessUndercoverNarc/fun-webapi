using System.ComponentModel.DataAnnotations;
using Models.Attributes;

namespace Models.DTOs.Login
{
    public class LoginDto
    {
        [Required]
        [String(1, 32)]
        public string Login { get; set; }
        
        [Required]
        [String(1, 32)]
        public string Password { get; set; }
    }
}