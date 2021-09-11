using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Tree;

namespace Models.Db.Relations
{
    public class FolderShare
    {
        [ForeignKey(nameof(FunAccount))]
        public long FunAccountId { get; set; }

        public virtual FunAccount FunAccount { get; set; }

        [ForeignKey(nameof(Folder))]
        public long FolderId { get; set; }

        public virtual Folder Folder { get; set; }

        public bool HasWriteAccess { get; set; }
    }
}