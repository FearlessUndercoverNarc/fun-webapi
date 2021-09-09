using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public class CardConnection : IdEntity
    {
        [ForeignKey(nameof(CardLeft))]
        public long CardLeftId { get; set; }

        public virtual Card CardLeft { get; set; }

        [ForeignKey(nameof(CardRight))]
        public long CardRightId { get; set; }

        public virtual Card CardRight { get; set; }
    }
}