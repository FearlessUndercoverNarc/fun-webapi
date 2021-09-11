using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.DTOs.Desks;
using Models.Misc;
using Services.External;
using Services.SharedServices.Abstractions;
using Services.Versioned.V1;
using Services.Versioned.V2;

namespace Services.Versioned.Implementations
{
    public partial class DeskTrashBinService : IDeskTrashBinServiceV2
    {
        async Task<ICollection<DeskWithIdDto>> IDeskTrashBinServiceV2.GetMyTrashBin()
        {
            var requestAccountId = _requestAccountIdService.Id;

            var desks = await _deskRepository.GetMany(
                d => d.AuthorAccountId == requestAccountId && d.IsInTrashBin && !d.Parent.IsInTrashBin,
                d => d.Parent,
                d => d.AuthorAccount
            );

            var deskWithIdDtos = _mapper.Map<ICollection<DeskWithIdDto>>(desks);

            return deskWithIdDtos;
        }

        async Task IDeskTrashBinServiceV2.MoveToTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.MoveToTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете переместить эту доску в корзину, т.к. не являетесь её владельцем!");
            }

            if (desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.MoveToTrashBin:\nAttempt to access desk in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент уже в корзине");
            }

            desk.IsInTrashBin = true;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskTrashBinServiceV2.RestoreFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RestoreFromTrashBin:\nAttempt to access restricted desk!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете восстановить данную доску из корзины, т.к. не являетесь её владельцем!");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RestoreFromTrashBin:\nAttempt to access desk not in trash bin!\nFolderId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Элемент не в корзине");
            }

            desk.IsInTrashBin = false;
            desk.LastUpdatedAt = DateTime.Now;

            await _deskRepository.Update(desk);
        }

        async Task IDeskTrashBinServiceV2.RemoveFromTrashBin(long id)
        {
            var desk = await _deskRepository.GetById(id);

            if (desk.AuthorAccountId != _requestAccountIdService.Id)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RemoveFromTrashBin:\nAttempt to access restricted folder!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Вы не можете удалять этот элемент");
            }

            if (!desk.IsInTrashBin)
            {
                await TelegramAPI.Send($"IDeskServiceV2.RemoveFromTrashBin:\nAttempt to remove element not in trash bin!\nDeskId ({id})\nUser ({_requestAccountIdService.Id})");
                throw new FunException("Удаление элементов вне корзины запрещено");
            }

            desk.LastUpdatedAt = DateTime.Now;
            await _deskRepository.Remove(desk);
        }
    }
}