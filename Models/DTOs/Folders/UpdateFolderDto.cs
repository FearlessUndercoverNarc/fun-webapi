using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Tree;

namespace Models.DTOs.Folders
{
    public class UpdateFolderDto
    {
        [Required]
        [Id(typeof(Folder))]
        public long Id { get; set; }
        
        [Required]
        [String(1, 256)]
        public string Title { get; set; }
    }
}