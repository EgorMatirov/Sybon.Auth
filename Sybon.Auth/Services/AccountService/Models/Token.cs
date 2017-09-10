namespace Sybon.Auth.Services.AccountService.Models
{
    public class Token
    {
        public long UserId { get; set; }
        public string Key { get; set; }
        public long? ExpiresIn { get; set; }
    }
}