using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public enum ActionType
    {
        DeskInit,
        DeskUpdate,
        CreateCard,
        Connect,
        Disconnect,
        DeleteCard,
        UpdateCard
    }

    public class DeskActionHistoryItem : VersionedEntity
    {
        [ForeignKey(nameof(Desk))]
        public long DeskId { get; set; }

        public virtual Desk Desk { get; set; }

        [ForeignKey(nameof(FunAccount))]
        public long FunAccountId { get; set; }

        public virtual FunAccount FunAccount { get; set; }

        public ActionType Action { get; set; }

        public string OldData { get; set; }

        public string NewData { get; set; }
    }
}