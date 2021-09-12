using System.Collections.Generic;

namespace Models.ImportExport
{
    public class DeskModel
    {
        public string Title { get; set; }
        
        public string Description { get; set; }

        public virtual List<CardModel> Cards { get; set; }
        
        public virtual List<CardConnectionModel> CardConnections { get; set; }
    }
}