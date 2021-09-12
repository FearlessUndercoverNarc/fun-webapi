using System.Text;
using System.Threading.Tasks;
using Models.DTOs.Misc;
using Models.ImportExport;
using Newtonsoft.Json;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class FolderImportExportService : IFolderImportExportServiceV2
    {
        async Task<(byte[] encodedData, string title)> IFolderImportExportServiceV2.Export(long id)
        {
            var folder = await _folderRepository.GetById(id);

            var folderModel = await CollectModelRecursive(id, new IntHolder());

            var jsonModel = JsonConvert.SerializeObject(folderModel, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            });

            var bytes = Encoding.UTF8.GetBytes(jsonModel);

            // TODO: Sign RSA and encode

            return (bytes, folder.Title);
        }

        async Task<CreatedDto> IFolderImportExportServiceV2.Import(byte[] data, long? parentId)
        {
            // TODO: Verify parentId folder belongs to same account

            var jsonModel = Encoding.UTF8.GetString(data);
            var folderModel = JsonConvert.DeserializeObject<FolderModel>(jsonModel);

            var folder = await UnwrapModelRecursive(folderModel, parentId);

            return folder.Id;
        }
    }
}