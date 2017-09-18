using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Auth.Services.AccountService;
using Sybon.Auth.Services.AccountService.Models;
using Sybon.Auth.Services.UsersService;
using User = Sybon.Auth.Services.UsersService.Models.User;

namespace Sybon.Auth.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet("auth")]
        [SwaggerOperation("Auth")]
        [SwaggerResponse((int) HttpStatusCode.OK, typeof(Token))]
        public async Task<IActionResult> Auth(
            [FromServices] IAccountService accountService,
            [FromQuery] string login,
            [FromQuery] string password
        )
        {
            var token = await accountService.AuthAsync(login, password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }

        [HttpPost("reg")]
        [SwaggerOperation("Registrate")]
        public async Task<IActionResult> Registrate(
            [FromServices] IUsersService usersService,
            [FromBody] UserModel user
        )
        {
            if (user == null || !TryValidateModel(user))
            {
                return BadRequest();
            }
            var userModel = new User
            {
                Name = user.Name,
                Login = user.Login,
                Password = user.Password,
                Role = user.Login == "admin" ? Services.UsersService.Models.User.RoleType.Admin : Services.UsersService.Models.User.RoleType.User // TODO: remove
            };
            await usersService.AddAsync(userModel);
            return Ok();
        }

        [HttpGet("check")]
        [SwaggerOperation("CheckToken")]
        [SwaggerResponse((int) HttpStatusCode.OK, typeof(Token))]
        public async Task<IActionResult> CheckToken(
            [FromServices] IAccountService accountService,
            // ReSharper disable once InconsistentNaming
            [FromQuery] string api_key
        )
        {
            var token = await accountService.CheckTokenAsync(api_key);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
        
        public class UserModel
        {
            [Required] public string Name { get; [UsedImplicitly] set; }
            [Required] public string Login { get; [UsedImplicitly] set; }
            [Required] public string Password { get; [UsedImplicitly] set; }
        }
    }
}