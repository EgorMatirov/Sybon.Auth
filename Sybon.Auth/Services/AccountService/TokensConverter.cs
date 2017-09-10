using JetBrains.Annotations;
using Sybon.Auth.Services.AccountService.Models;

namespace Sybon.Auth.Services.AccountService
{
    [UsedImplicitly]
    public class TokensConverter : ITokensConverter
    {
        public Token Convert(Repositories.TokensRepository.Entities.Token token)
        {
            if (token == null) return null;
            return new Token
            {
                UserId = token.UserId,
                ExpiresIn = token.ExpireTime?.Ticks,
                Key = token.Key
            };
        }
    }
}