namespace Sybon.Auth.Services.PasswordsService
{
    public interface IPasswordsService
    {
        string HashPassword(string password);
    }
}