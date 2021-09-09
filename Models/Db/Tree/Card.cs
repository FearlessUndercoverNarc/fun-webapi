using System.ComponentModel.DataAnnotations.Schema;
using Models.Db.Common;

namespace Models.Db.Tree
{
    public class Card : IdEntity
    {
        public string Title { get; set; }
        
        [ForeignKey(nameof(Desk))]
        public long DeskId { get; set; }

        public virtual Desk Desk { get; set; }
    }
}