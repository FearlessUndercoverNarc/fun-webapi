namespace Models.DTOs.CardConnections
{
    public class CardConnectionWithIdDto
    {
        public long Id { get; set; }
        
        public long CardLeftId { get; set; }

        public long CardRightId { get; set; }
    }
}