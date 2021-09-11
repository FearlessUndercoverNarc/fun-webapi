using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Tree;

namespace Models.DTOs.Desks
{
    public class CreateDeskDto
    {
        [Required]
        [String(1, 256)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        [String(0, 512)]
        public string Description { get; set; }
        
        [Required]
        [Id(typeof(Folder))]
        public long ParentId { get; set; }
    }
}