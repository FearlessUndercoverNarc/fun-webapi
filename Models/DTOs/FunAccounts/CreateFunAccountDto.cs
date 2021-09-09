using System.ComponentModel.DataAnnotations;
using Models.Attributes;

namespace Models.DTOs.FunAccounts
{
    public class CreateFunAccountDto
    {
        [Required]
        [String(1, 32)]
        public string Login { get; set; }

        [Required]
        [String(1, 256)]
        public string Fio { get; set; }

        [Required]
        [String(1, 32)]
        public string Password { get; set; }
    }
}