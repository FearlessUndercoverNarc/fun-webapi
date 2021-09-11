using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs.FunAccounts;
using Models.DTOs.Misc;

namespace Services.SharedServices.Abstractions
{
    public interface IFunAccountService
    {
        Task<CreatedDto> CreateFunAccount(CreateFunAccountDto createFunAccountDto);
        
        Task UpdateFunAccount(UpdateFunAccountDto updateFunAccountDto);

        Task<ICollection<FunAccountWithIdDto>> GetAll();
    }
}