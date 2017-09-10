using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sybon.Auth.ApiStubs;
using Sybon.Auth.Services.UsersService.Models;

namespace Sybon.Auth
{
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        private readonly User.RoleType _userRole;

        public PermissionFilterAttribute(User.RoleType userRole)
        {
            _userRole = userRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!(context.Controller is ILogged controller))
                throw new Exception("Controller must derive from ILogged");

            var permissionsApi = (IPermissionsApi) context.HttpContext.RequestServices.GetService(typeof(IPermissionsApi));
            var userRole = permissionsApi.GetUserRoleAsync(controller.UserId).Result;
            if (userRole != _userRole)
                context.Result = new UnauthorizedResult();
        }
    }
}