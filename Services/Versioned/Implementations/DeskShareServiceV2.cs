using System.Threading.Tasks;
using Models.Db.Relations;
using Models.Misc;
using Services.External;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class DeskShareService : IDeskShareServiceV2
    {
        async Task IDeskShareServiceV2.Share(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.Share:\nAttempt to access restricted desk!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете управлять доступом к этой доске, так как не являетесь её владельцем.");
            }

            if (await _folderShareRepository.IsSharedTo(desk.ParentId, requestAccountId))
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.Share:\nAttempt to share a desk in a shared folder!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Папка с этой доской уже доступна этому пользователю"); 
            }

            if (await _deskShareRepository.IsSharedTo(id, recipientId))
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.Share:\nAttempt to share already shared desk!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Данная доска уже доступна этому пользователю");
            }

            var deskShare = new DeskShare
            {
                DeskId = id,
                FunAccountId = recipientId
            };

            await _deskShareRepository.Add(deskShare);
        }

        async Task IDeskShareServiceV2.RemoveShare(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.RemoveShare:\nAttempt to access restricted desk!\nDeskId ({id})\nUser ({requestAccountId})");
                throw new FunException("Вы не можете управлять доступом к этой доске, так как не являетесь её владельцем.");
            }

            if (!await _deskShareRepository.IsSharedTo(id, recipientId))
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.RemoveShare:\nAttempt to remove share from unshared desk!\nDeskId ({id})\nUser ({requestAccountId})");
                throw new FunException("Данная доска недоступна этому пользователю");
            }

            var deskShare = await _deskShareRepository.GetOne(s => s.DeskId == id && s.FunAccountId == recipientId);

            await _deskShareRepository.Remove(deskShare);
        }
    }
}