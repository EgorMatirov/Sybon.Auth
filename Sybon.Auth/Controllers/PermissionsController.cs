using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Auth.Services.PermissionsService;
using Sybon.Auth.Services.PermissionsService.Models;
using Sybon.Auth.Services.UsersService;
using Sybon.Auth.Services.UsersService.Models;
using Sybon.Common;

namespace Sybon.Auth.Controllers
{
    [Route("api/[controller]")]
    public class PermissionsController : Controller, ILogged
    {
        [HttpGet("{userId}/collections/{collectionId}")]
        [SwaggerOperation("GetToCollection")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PermissionType))]
        public async Task<IActionResult> GetToCollection([FromServices] IPermissionsService permissionsService, long userId, long collectionId)
        {
            var permission = await permissionsService.GetToCollectionAsync(userId, collectionId);
            return Ok(permission.ToString());
        }

        [HttpPost("{userId}/collections/{collectionId}")]
        [SwaggerOperation("AddToCollection")]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        [PermissionFilter(Services.UsersService.Models.User.RoleType.Admin)]
        public async Task<IActionResult> AddToCollection([FromServices] IPermissionsService permissionsService, long userId, long collectionId, PermissionType permission)
        {
            await permissionsService.AddToCollectionAsync(userId, collectionId, permission);
            return Ok();
        }

        [HttpGet("{userId}/problems/{problemId}")]
        [SwaggerOperation("GetToProblem")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PermissionType))]
        public async Task<IActionResult> GetToProblem([FromServices] IPermissionsService permissionsService, long userId, long problemId)
        {
            var permission = await permissionsService.GetToProblemAsync(userId, problemId);
            return Ok(permission.ToString());
        }

        [HttpPost("{userId}/requestsCount/tryIncreaseBy")]
        [SwaggerOperation("TryIncreaseRequestsCountBy")]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(bool))]
        [AuthorizeFilter]
        [PermissionFilter(Services.UsersService.Models.User.RoleType.Admin)]
        public IActionResult TryIncreaseRequestsCountBy([FromServices] IPermissionsService permissionsService, long userId, [FromBody] int cnt)
        {
            var result = permissionsService.TryIncreaseSubmitsCount(userId, cnt);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        [SwaggerOperation("GetUserRole")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(User.RoleType))]
        [EnumDataType(typeof(User.RoleType))]
        public async Task<IActionResult> GetUserRole([FromServices] IUsersService usersService, long userId)
        {
            var userRole = await usersService.GetUserRoleAsync(userId);
            return Ok(userRole.ToString());
        }
        
        public long UserId { get; set; }
        public string ApiKey { get; set; }
    }
}