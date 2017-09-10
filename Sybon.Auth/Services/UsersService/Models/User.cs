namespace Sybon.Auth.Services.UsersService.Models
{
    public class User
    {
        public enum RoleType
        {
            User = 0,
            Admin = 1
        }
        
        public long Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long? TokenId { get; set; }
        public RoleType Role { get; set; }
        //public Token Token { get; set; }
    }
}