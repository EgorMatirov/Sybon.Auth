﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sybon.Auth
{
    public class SwaggerApiKeySecurityFilter : IOperationFilter
    {
        private readonly IOptions<AuthorizationOptions> _authorizationOptions;

        public SwaggerApiKeySecurityFilter(IOptions<AuthorizationOptions> authorizationOptions)
        {
            _authorizationOptions = authorizationOptions;
        }

        public SwaggerApiKeySecurityFilter()
        {
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    {"api_key", new string[0]}
                }
            };
//            }
        }
    }
}