using System;
using Microsoft.AspNetCore.Hosting;
using Sybon.Common;

namespace Sybon.Auth
{
    public class AuthService : BaseService
    {
        public AuthService(Func<string[], IWebHost> buildWebHostFunc) : base(buildWebHostFunc)
        {
        }

        public override string ServiceName => "Sybon.Auth";
    }
}