using System.ComponentModel.DataAnnotations;
using Models.Attributes;
using Models.Db.Tree;

namespace Models.DTOs.Folders
{
    public class CreateFolderDto
    {
        [Required]
        [String(1, 256)]
        public string Title { get; set; }

        [Id(typeof(Folder))]
        public long? ParentId { get; set; }
    }
}