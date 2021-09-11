using Models.Db.Tree;

namespace Models.DTOs
{
    public class DeskActionDto
    {
        public long DeskId { get; set; }
        
        public long FunAccountId { get; set; }

        public ActionType Action { get; set; }

        public string OldData { get; set; }

        public string NewData { get; set; }
    }
}