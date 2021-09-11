namespace Models.DTOs.Relations
{
    public class FolderShareDto
    {
        public long FunAccountId { get; set; }

        public bool HasWriteAccess { get; set; }
    }
}