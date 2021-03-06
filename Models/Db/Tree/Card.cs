using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public class Card : IdEntity
    {
        public uint X { get; set; }
        public uint Y { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        [MaxLength(40)]
        public string Image { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        [MaxLength(2048)]
        public string ExternalUrl { get; set; }

        // Example: #AABBCCDD
        [MaxLength(9)]
        public string ColorHex { get; set; }

        [ForeignKey(nameof(Desk))]
        public long DeskId { get; set; }

        public virtual Desk Desk { get; set; }

        public virtual ICollection<Card> AsLeftCards { get; set; }

        public virtual ICollection<CardConnection> AsLeftCardConnections { get; set; }

        public virtual ICollection<Card> AsRightCards { get; set; }

        public virtual ICollection<CardConnection> AsRightCardConnections { get; set; }
    }
}