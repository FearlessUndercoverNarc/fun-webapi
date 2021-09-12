using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Tree;
using Models.DTOs.Misc;
using Models.ImportExport;
using Newtonsoft.Json;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;

namespace Services.Versioned.Implementations
{
    public partial class FolderImportExportService : IFolderImportExportServiceV1
    {
        private IFolderRepository _folderRepository;
        private IDeskRepository _deskRepository;
        private ICardRepository _cardRepository;
        private ICardConnectionRepository _cardConnectionRepository;
        private IRequestAccountIdService _requestAccountIdService;

        private IMapper _mapper;

        public FolderImportExportService(IFolderRepository folderRepository, IMapper mapper, IDeskRepository deskRepository, IRequestAccountIdService requestAccountIdService, ICardRepository cardRepository, ICardConnectionRepository cardConnectionRepository)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _deskRepository = deskRepository;
            _requestAccountIdService = requestAccountIdService;
            _cardRepository = cardRepository;
            _cardConnectionRepository = cardConnectionRepository;
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
            var folder = await _folderRepository.GetById(id);

            var folderModel = await CollectModelRecursive(id, new IntHolder());

            var jsonModel = JsonConvert.SerializeObject(folderModel, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            });

            var bytes = Encoding.UTF8.GetBytes(jsonModel);

            var aeskey = Encoding.UTF8.GetBytes("i am as cool as my ass");
            
            //Объявляем объект класса AES
            Aes aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
            aes.Key = aeskey;


            await using MemoryStream srcStream = new(bytes);
            var crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            await using var destStream = new MemoryStream();
            await using var cryptoStream = new CryptoStream(srcStream, crypt, CryptoStreamMode.Write);
            await cryptoStream.CopyToAsync(destStream);
            var encrypted = destStream.ToArray();

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

        async Task<CreatedDto> IFolderImportExportServiceV1.Import(byte[] data, long? parentId)
        {
            // TODO: Verify parentId folder belongs to same account

            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[data.Length - 16];
            //Списываем соль
            for (int i = data.Length - 16, j = 0; i < data.Length; i++, j++)
                bytesIv[j] = data[i];
            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < data.Length - 16; i++)
                mess[i] = data[i];
            
            var aeskey = Encoding.UTF8.GetBytes("egop is super cool");
            
            //Объект класса Aes
            Aes aes = Aes.Create();
            //Задаем тот же ключ, что и для шифрования
            aes.Key = aeskey;
            //Задаем соль
            aes.IV = bytesIv;
            //Строковая переменная для результата
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            await using MemoryStream srcStream = new MemoryStream(mess);
            await using CryptoStream cryptoStream = new CryptoStream(srcStream, crypt, CryptoStreamMode.Read);
            await cryptoStream.CopyToAsync(srcStream);

            byte[] decrypted = srcStream.ToArray();

            var jsonModel = Encoding.UTF8.GetString(decrypted);
            var folderModel = JsonConvert.DeserializeObject<FolderModel>(jsonModel);

            var folder = await UnwrapModelRecursive(folderModel, parentId);

            return folder.Id;
        }
    }
}