using System.Threading.Tasks;
using Models.DTOs.Misc;

namespace Services.Versioned.V2
{
    public interface IFolderImportExportServiceV2
    {
        Task<(byte[] encodedData, string title)> Export(long id);
        
        Task<CreatedDto> Import(byte[] data, long? parentId);
    }
}