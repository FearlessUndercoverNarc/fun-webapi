using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Account;
using Models.Db.Common;
using Models.Db.Relations;

namespace Models.Db.Tree
{
    public class Desk : IdEntity
    {
        [MaxLength(256)]
        public string Title { get; set; }
        
        [MaxLength(512)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [ForeignKey(nameof(AuthorAccount))]
        public long AuthorAccountId { get; set; }

        public virtual FunAccount AuthorAccount { get; set; }
        
        public virtual ICollection<FunAccount> SharedTo { get; set; }
        public virtual ICollection<DeskShare> SharedToRelation { get; set; }

        public bool IsInTrashBin { get; set; }

        public virtual ICollection<DeskActionHistoryItem> HistoryItems { get; set; }
        
        [ForeignKey(nameof(Parent))]
        public long ParentId { get; set; }

        public virtual Folder Parent { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

        public virtual ICollection<CardConnection> CardConnections { get; set; }
    }
}