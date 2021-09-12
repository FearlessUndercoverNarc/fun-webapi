using System.Threading.Tasks;
using Models.DTOs.Misc;

namespace Services.Versioned.V1
{
    public interface IFolderImportExportServiceV1
    {
        Task<(byte[] encodedData, string title)> Export(long id);
        
        Task<CreatedDto> Import(byte[] data, long? parentId);
    }
}