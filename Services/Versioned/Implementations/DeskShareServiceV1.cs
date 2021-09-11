using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Models.Db.Relations;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class DeskShareService : IDeskShareServiceV1
    {
        private IFolderRepository _folderRepository;
        private IDeskRepository _deskRepository;
        private IDeskShareRepository _deskShareRepository;
        private IRequestAccountIdService _requestAccountIdService;

        public DeskShareService(IFolderRepository folderRepository, IDeskRepository deskRepository, IDeskShareRepository deskShareRepository, IRequestAccountIdService requestAccountIdService)
        {
            _folderRepository = folderRepository;
            _deskRepository = deskRepository;
            _deskShareRepository = deskShareRepository;
            _requestAccountIdService = requestAccountIdService;
        }

        async Task IDeskShareServiceV1.Share(long id, long recipientId)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != requestAccountId)
            {
                await TelegramAPI.Send($"IDeskShareServiceV1.Share:\nAttempt to access restricted desk!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете управлять доступом к этой доске, так как не являетесь её владельцем.");
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

        async Task IDeskShareServiceV1.RemoveShare(long id, long recipientId)
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