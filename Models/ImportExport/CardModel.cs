﻿namespace Models.ImportExport
{
    public class CardModel
    {
        public long TempId { get; set; }
        
        public uint X { get; set; }
        public uint Y { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }

        public string ColorHex { get; set; }
    }
}