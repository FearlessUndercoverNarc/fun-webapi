using System;
using System.Collections.Generic;
using Models.DTOs.Relations;

namespace Models.DTOs.Desks
{
    public class DeskWithIdDto
    {
        public long Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public long AuthorAccountId { get; set; }

        public string AuthorAccountFio { get; set; }
        
        public long ParentId { get; set; }

        public string ParentTitle { get; set; }
    }
}