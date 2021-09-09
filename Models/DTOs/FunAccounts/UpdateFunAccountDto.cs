using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Account;

namespace Models.DTOs.FunAccounts
{
    public class UpdateFunAccountDto
    {
        [Required]
        [String(1, 256)]
        public string Fio { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 32)]
        public string Password { get; set; }
    }
}