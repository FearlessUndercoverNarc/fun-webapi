using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Tree;

namespace Models.Db.Relations
{
    public class DeskShare
    {
        [ForeignKey(nameof(FunAccount))]
        public long FunAccountId { get; set; }

        public virtual FunAccount FunAccount { get; set; }

        [ForeignKey(nameof(Desk))]
        public long DeskId { get; set; }

        public virtual Desk Desk { get; set; }

        public bool HasWriteAccess { get; set; }
    }
}