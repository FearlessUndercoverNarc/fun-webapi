using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public class DeskActionHistoryItem : IdEntity
    {
        [ForeignKey(nameof(Desk))]
        public long DeskId { get; set; }

        public virtual Desk Desk { get; set; }

        [ForeignKey(nameof(FunAccount))]
        public long FunAccountId { get; set; }

        public virtual FunAccount FunAccount { get; set; }

        public string Action { get; set; }

        public DateTime DateTime { get; set; }

        enum Actions
        {
            CreateCard,
            Connect,
            Disconnect,
            DeleteCard,
            UpdateCard
        }
        
        // Create card with full info, including X and Y
        // Connect from left (first) to right (second)
        // Delete with connectionId
        // Update card with all fields
        // {"Action":"CreateCard", "Data": ["X", "Y", "TITLE"] }
    }
}