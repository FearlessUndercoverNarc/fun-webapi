using System.Collections.Generic;
using Models.DTOs.Desks;

namespace Models.DTOs.Folders
{
    public class FolderWithIdDto
    {
        public string Title { get; set; }

        public long AuthorAccountId { get; set; }

        public long? ParentId { get; set; }

        public ICollection<DeskWithIdDto> Desks { get; set; }
    }
}