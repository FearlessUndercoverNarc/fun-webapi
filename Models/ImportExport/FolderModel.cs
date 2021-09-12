using System.Collections.Generic;

namespace Models.ImportExport
{
    public class FolderModel
    {
        public string Title { get; set; }

        public virtual List<FolderModel> Children { get; set; }
        
        public virtual List<DeskModel> Desks { get; set; }
    }
}