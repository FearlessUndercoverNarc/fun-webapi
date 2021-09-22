using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Misc;
using Models.ImportExport;
using Models.Misc;
using Newtonsoft.Json;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class FolderImportExportService : IFolderImportExportServiceV1
    {
        private static readonly byte[] AesKey = Encoding.UTF8.GetBytes("Mr Egop is as awesome as his ass");
        private IFolderRepository _folderRepository;
        private IDeskRepository _deskRepository;
        private ICardRepository _cardRepository;
        private ICardConnectionRepository _cardConnectionRepository;
        private IRequestAccountIdService _requestAccountIdService;

        private IMapper _mapper;
        private IFolderShareRepository _folderShareRepository;

        public FolderImportExportService(IFolderRepository folderRepository, IMapper mapper, IDeskRepository deskRepository, IRequestAccountIdService requestAccountIdService, ICardRepository cardRepository, ICardConnectionRepository cardConnectionRepository, IFolderShareRepository folderShareRepository)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _deskRepository = deskRepository;
            _requestAccountIdService = requestAccountIdService;
            _cardRepository = cardRepository;
            _cardConnectionRepository = cardConnectionRepository;
            _folderShareRepository = folderShareRepository;
        }

        class IntHolder
        {
            public int value;
        }

        private async Task<FolderModel> CollectModelRecursive(long id, IntHolder uniqueCardId)
        {
            var folder = await _folderRepository.GetById(
                id,
                f => f.Children,
                f => f.Desks
            );

            var folderModel = _mapper.Map<FolderModel>(folder);

            folderModel.Children = new List<FolderModel>();

            foreach (var child in folder.Children)
            {
                folderModel.Children.Add(await CollectModelRecursive(child.Id, uniqueCardId));
            }

            folderModel.Desks = new List<DeskModel>();
            foreach (var folderDesk in folder.Desks)
            {
                var desk = await _deskRepository.GetById(
                    folderDesk.Id,
                    d => d.Cards,
                    d => d.CardConnections
                );

                var deskModel = _mapper.Map<DeskModel>(desk);
                deskModel.Cards = new List<CardModel>();

                Dictionary<long, long> cardIdTransformMap = new();

                foreach (var deskCard in desk.Cards)
                {
                    var tempId = uniqueCardId.value++;
                    cardIdTransformMap.Add(deskCard.Id, tempId);

                    var cardModel = _mapper.Map<CardModel>(deskCard);
                    cardModel.TempId = tempId;
                    deskModel.Cards.Add(cardModel);
                }

                foreach (var deskCardConnection in desk.CardConnections)
                {
                    var cardConnectionModel = new CardConnectionModel()
                    {
                        CardLeft = cardIdTransformMap[deskCardConnection.CardLeftId],
                        CardRight = cardIdTransformMap[deskCardConnection.CardRightId]
                    };

                    deskModel.CardConnections.Add(cardConnectionModel);
                }

                folderModel.Desks.Add(deskModel);
            }

            return folderModel;
        }

        async Task<(byte[] encodedData, string title)> IFolderImportExportServiceV1.Export(long id)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var folder = await _folderRepository.GetById(id);
            
            if (!(folder.AuthorAccountId == requestAccountId || await _folderShareRepository.HasSharedReadTo(folder.Id, requestAccountId)))
            {
                await TelegramAPI.Send($"IFolderImportExportServiceV1.Export:\nAttempt to access restricted folder!\nFolderId ({id})\nUser ({requestAccountId})");
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

        private async Task<Folder> UnwrapModelRecursive(FolderModel model, long? parentId)
        {
            var requestAccountId = _requestAccountIdService.Id;

            var folder = _mapper.Map<Folder>(model);
            folder.CreatedAt = DateTime.Now;
            folder.LastUpdatedAt = DateTime.Now;
            folder.AuthorAccountId = requestAccountId;
            folder.ParentId = parentId;

            await _folderRepository.Add(folder);

            foreach (var modelChild in model.Children)
            {
                await UnwrapModelRecursive(modelChild, folder.Id);
            }

            foreach (var modelDesk in model.Desks)
            {
                var desk = _mapper.Map<Desk>(modelDesk);
                desk.AuthorAccountId = requestAccountId;
                desk.CreatedAt = DateTime.Now;
                desk.LastUpdatedAt = DateTime.Now;
                desk.ParentId = folder.Id;

                await _deskRepository.Add(desk);

                Dictionary<long, long> tempIdToRealIdMap = new();
                List<Card> cards = new(modelDesk.Cards.Count);

                foreach (var modelCard in modelDesk.Cards)
                {
                    var card = _mapper.Map<Card>(modelCard);
                    card.DeskId = desk.Id;
                    cards.Add(card);
                }

                await _cardRepository.AddMany(cards);

                for (var i = 0; i < cards.Count; i++)
                {
                    tempIdToRealIdMap.Add(modelDesk.Cards[i].TempId, cards[i].Id);
                }

                List<CardConnection> cardConnections = new(modelDesk.CardConnections.Count);
                
                foreach (var modelCardConnection in modelDesk.CardConnections)
                {
                    var cardConnection = new CardConnection()
                    {
                        CardLeftId = tempIdToRealIdMap[modelCardConnection.CardLeft],
                        CardRightId = tempIdToRealIdMap[modelCardConnection.CardRight]
                    };

                    cardConnections.Add(cardConnection);
                }
                
                await _cardConnectionRepository.AddMany(cardConnections);
            }

            return folder;
        }
        
        /// <summary>
        /// Шифрует исходное сообщение AES ключом (добавляет соль)
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static byte[] ToAes256(string src)
        {
            //Объявляем объект класса AES
            Aes aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
            aes.Key = AesKey;
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(src);
                    }
                }
                //Записываем в переменную encrypted зашиврованный поток байтов
                encrypted = ms.ToArray();
            }
            //Возвращаем поток байт + крепим соль
            return encrypted.Concat(aes.IV).ToArray();
        }
        
        /// <summary>
        /// Расшифровывает криптованного сообщения
        /// </summary>
        /// <param name="shifr">Шифротекст в байтах</param>
        /// <returns>Возвращает исходную строку</returns>
        public static string FromAes256(byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            //Списываем соль
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                bytesIv[j] = shifr[i];
            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < shifr.Length - 16; i++)
                mess[i] = shifr[i];
            //Объект класса Aes
            Aes aes = Aes.Create();
            //Задаем тот же ключ, что и для шифрования
            aes.Key = AesKey;
            //Задаем соль
            aes.IV = bytesIv;
            //Строковая переменная для результата
            string text = "";
            byte[] data = mess;
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        //Результат записываем в переменную text в вие исходной строки
                        text = sr.ReadToEnd();
                    }
                }
            }
            return text;
        }
        
        async Task<CreatedDto> IFolderImportExportServiceV1.Import(byte[] encryptedData, long? parentId)
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

            var jsonModel = Encoding.UTF8.GetString(encryptedData); // FromAes256(encryptedData);
            var folderModel = JsonConvert.DeserializeObject<FolderModel>(jsonModel);

            var folder = await UnwrapModelRecursive(folderModel, parentId);

            return folder.Id;
        }
    }
}