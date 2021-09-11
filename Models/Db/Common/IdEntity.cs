using System.ComponentModel.DataAnnotations;

namespace Models.Db.Common
{
    public class IdEntity
    {
        [Key]
        public long Id { get; set; }

        public bool IsSoftDeleted { get; set; }
    }
}