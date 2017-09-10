using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace Sybon.Auth.Services.PasswordsService
{
    [UsedImplicitly]
    public class PasswordsService : IPasswordsService
    {
        public string HashPassword(string password)
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sBuilder = new StringBuilder();
                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}