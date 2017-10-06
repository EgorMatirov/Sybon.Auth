using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sybon.Auth.ApiStubs;

namespace Sybon.Auth
{
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var apiKey = context.HttpContext.Request.Query["api_key"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var accountApi = (IAccountApi) context.HttpContext.RequestServices.GetService(typeof(IAccountApi));
            var token = accountApi.CheckTokenAsync(apiKey).Result;
            if (token == null || token.ExpiresIn != null && token.ExpiresIn < DateTime.UtcNow.Ticks)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!(context.Controller is ILogged controller))
                throw new Exception("Controller must derive from ILogged");
            controller.UserId = token.UserId;
            controller.ApiKey = token.Key;
        }
    }

    public interface ILogged
    {
        long UserId { get; set; }
        string ApiKey { get; set; }
    }
}