using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Tree;

namespace Models.DTOs.Cards
{
    public class UpdateCardDto
    {
        [Required]
        [Id(typeof(Card))]
        public long Id { get; set; }
        
        [Required]
        [Range(0, 10000)]
        public uint X { get; set; }

        [Required]
        [Range(0, 10000)]
        public uint Y { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 128)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 40)]
        public string Image { get; set; }

        [Required]
        [String(1, 512)]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 2048)]
        public string ExternalUrl { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 9)]
        public string ColorHex { get; set; }

        [Required]
        [Id(typeof(Desk))]
        public long DeskId { get; set; }
    }
}