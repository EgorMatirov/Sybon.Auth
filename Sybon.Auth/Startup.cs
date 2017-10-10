using System.Collections.Generic;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Sybon.Archive.Client.Api;
using Sybon.Archive.Client.Client;
using Sybon.Auth.ApiStubs;
using Sybon.Auth.Repositories.CollectionPermissionsRepository;
using Sybon.Auth.Repositories.SubmitLimitsRepository;
using Sybon.Auth.Repositories.TokensRepository;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Auth.Repositories.UsersRepository;
using Sybon.Auth.Repositories.UsersRepository.Entities;
using Sybon.Auth.Services.AccountService;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Auth.Services.PermissionsService;
using Sybon.Auth.Services.UsersService;
using Sybon.Common;

namespace Sybon.Auth
{
    [UsedImplicitly]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFrameworkInMemoryDatabase();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Sybon.Auth", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.AddSecurityDefinition("api_key", new ApiKeyScheme {In = "query", Name = "api_key"});
                c.OperationFilter<SwaggerApiKeySecurityFilter>();
            });

            services.AddDbContext<AuthContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IRepositoryUnitOfWork, RepositoryUnitOfWork<AuthContext>>();

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersService, UsersService>();
            
            services.AddScoped<ITokensRepository, TokensRepository>();
            services.AddScoped<IAccountService, AccountService>();
            
            services.AddScoped<IPasswordsService, PasswordsService>();
            
            services.AddScoped<ICollectionPermissionsRepository, CollectionPermissionsRepository>();
            services.AddScoped<ISubmitLimitsRepository, SubmitLimitsRepository>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            
            services.AddScoped<IProblemsApi, ProblemsApi>();
            services.AddScoped<IAccountApi, AccountApi>();
            services.AddScoped<IPermissionsApi, PermissionsApi>();
            services.AddSingleton<IProblemsApi>(new ProblemsApi(new Configuration
            {
                BasePath = Configuration.GetConnectionString("Sybon.ArchiveUrl"),
                ApiKey = new Dictionary<string, string>
                {
                    {"api_key", Configuration.GetConnectionString("ApiKey")}
                }
            }));

            ConfigureMapper();
            services.AddSingleton(Mapper.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sybon.Auth V1");
            });

            app.UseMvc();
        }

        private static void ConfigureMapper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<User, Services.UsersService.Models.User>();
                config.CreateMap<Services.UsersService.Models.User, User>();
                
                config.
                    CreateMap<Token, Services.AccountService.Models.Token>()
                    .ForMember(dest => dest.ExpiresIn,
                        opt => opt.MapFrom(
                            src => src.ExpireTime == null ? null : (long?)src.ExpireTime.Value.Ticks
                        )
                    );
                config.CreateMap<Services.AccountService.Models.Token, Token>();
            });
        }
    }
}