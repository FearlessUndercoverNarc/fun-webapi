namespace Models.DTOs.Cards
{
    public class CardWithIdDto
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }

        public string ColorHex { get; set; }

        public long DeskId { get; set; }
    }
}