using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Common;
using Models.Db.Relations;

namespace Models.Db.Tree
{
    public class Folder : IdEntity
    {
        [MaxLength(256)]
        public string Title { get; set; }

        [ForeignKey(nameof(AuthorAccount))]
        public long AuthorAccountId { get; set; }

        public virtual FunAccount AuthorAccount { get; set; }

        // public virtual ICollection<FunAccount> SharedTo { get; set; }
        // public virtual ICollection<FolderShare> SharedToRelation { get; set; }

        public virtual ICollection<Folder> Children { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }

        [ForeignKey(nameof(Parent))]
        public long? ParentId { get; set; }

        public virtual Folder Parent { get; set; }

        public bool IsInTrashBin { get; set; }
    }
}