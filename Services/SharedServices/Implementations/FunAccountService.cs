using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Abstractions;
using Models.Db.Account;
using Models.DTOs.FunAccounts;
using Models.DTOs.Misc;

namespace Services.SharedServices.Abstractions
{
    public class FunAccountService : IFunAccountService
    {
        private IRequestAccountIdService _requestAccountIdService;
        private IFunAccountRepository _funAccountRepository;
        private IMapper _mapper;

        public FunAccountService(IFunAccountRepository funAccountRepository, IMapper mapper, IRequestAccountIdService requestAccountIdService)
        {
            _funAccountRepository = funAccountRepository;
            _mapper = mapper;
            _requestAccountIdService = requestAccountIdService;
        }

        public async Task<CreatedDto> CreateFunAccount(CreateFunAccountDto createFunAccountDto)
        {
            var funAccount = _mapper.Map<FunAccount>(createFunAccountDto);
            await _funAccountRepository.Add(funAccount);
            return funAccount.Id;
        }

        public async Task UpdateFunAccount(UpdateFunAccountDto updateFunAccountDto)
        {
            var requestAccountId = _requestAccountIdService.Id;
            var funAccount = await _funAccountRepository.GetById(requestAccountId);
            string oldPassword = funAccount.Password;
            _mapper.Map(updateFunAccountDto, funAccount);
            if (string.IsNullOrEmpty(updateFunAccountDto.Password))
            {
                // We don't change password here
                funAccount.Password = oldPassword;
            }

            await _funAccountRepository.Update(funAccount);
        }
    }
}