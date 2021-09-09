using System.Threading.Tasks;
using Models.Db.Account;
using Models.Db.Sessions;
using Models.DTOs.Login;
using Models.DTOs.Misc;

namespace Services.SharedServices.Abstractions
{
    public interface ITokenSessionService
    {
        Task<LoginResultDto> Login(LoginDto loginDto);

        Task<TokenSession> GetByToken(string token);
        
        Task<FunAccount> GetAccountByToken(string token);
        
        Task<long> GetAccountIdByToken(string token);
        
        Task Logout(string token);
    }
}