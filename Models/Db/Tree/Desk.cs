using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public class Desk : IdEntity
    {
        [ForeignKey(nameof(Folder))]
        public long FolderId { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

        public virtual ICollection<CardConnection> CardConnections { get; set; }
    }
}