using Sybon.Auth.Services.AccountService.Models;

namespace Sybon.Auth.Services.AccountService
{
    public interface ITokensConverter
    {
        Token Convert(Repositories.TokensRepository.Entities.Token token);
    }
}