using System.Text;
using System.Threading.Tasks;
using Models.DTOs.Misc;
using Models.ImportExport;
using Models.Misc;
using Newtonsoft.Json;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class FolderImportExportService : IFolderImportExportServiceV2
    {
        async Task<(byte[] encodedData, string title)> IFolderImportExportServiceV2.Export(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var folder = await _folderRepository.GetById(id);

            if (!(folder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedReadTo(folder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IFolderImportExportServiceV2.Export:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({requestAccountId})");
                throw new FunException("У вас нет доступа к этой папке");
            }

            var folderModel = await CollectModelRecursive(id, new IntHolder());

            var jsonModel = JsonConvert.SerializeObject(folderModel, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            });

            var encrypted = Encoding.UTF8.GetBytes(jsonModel); //ToAes256(jsonModel); 

            return (encrypted, folder.Title);
        }

        async Task<CreatedDto> IFolderImportExportServiceV2.Import(byte[] encryptedData, long? parentId)
        {
            var requestAccountId = _requestAccountIdService.Id;

            if (parentId is not null)
            {
                var parentFolder = await _folderRepository.GetById(parentId.Value);
                if (parentFolder.AuthorAccountId != requestAccountId)
                {
                    await TelegramAPI.Send($"IFolderImportExportServiceV1.Import:\nAttempt to access restricted folder!\nFolder ({parentFolder.Id}), Account({requestAccountId})");
                    throw new FunException("У вас нет доступа для импорта в эту папку");
                }
            }

            var jsonModel = Encoding.UTF8.GetString(encryptedData); //FromAes256(encryptedData);
            var folderModel = JsonConvert.DeserializeObject<FolderModel>(jsonModel);

            var folder = await UnwrapModelRecursive(folderModel, parentId);

            return folder.Id;
        }
    }
}