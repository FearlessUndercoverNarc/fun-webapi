using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Tree;

namespace Models.DTOs.CardConnections
{
    public class CreateCardConnectionDto
    {
        [Required]
        [Id(typeof(Card))]
        public long CardLeftId { get; set; }
        
        [Required]
        [Id(typeof(Card))]
        public long CardRightId { get; set; }
    }
}