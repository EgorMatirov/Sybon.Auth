﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Sybon.Auth.ApiStubs;
using Sybon.Auth.Repositories.CollectionPermissionsRepository;
using Sybon.Auth.Repositories.CollectionPermissionsRepository.Entities;
using Sybon.Auth.Repositories.SubmitLimitsRepository;
using Sybon.Auth.Repositories.SubmitLimitsRepository.Entities;
using Sybon.Auth.Repositories.TokensRepository;
using Sybon.Auth.Repositories.TokensRepository.Entities;
using Sybon.Auth.Repositories.UsersRepository;
using Sybon.Auth.Repositories.UsersRepository.Entities;
using Sybon.Auth.Services.AccountService;
using Sybon.Auth.Services.PasswordsService;
using Sybon.Auth.Services.PermissionsService;
using Sybon.Auth.Services.UsersService;

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

            AddDbContextPools(services);

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersConverter, UsersConverter>();
            services.AddScoped<IUsersService, UsersService>();
            
            services.AddScoped<ITokensRepository, TokensRepository>();
            services.AddScoped<ITokensConverter, TokensConverter>();
            services.AddScoped<IAccountService, AccountService>();
            
            services.AddScoped<IPasswordsService, PasswordsService>();
            
            services.AddScoped<ICollectionPermissionsRepository, CollectionPermissionsRepository>();
            services.AddScoped<ISubmitLimitsRepository, SubmitLimitsRepository>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            
            services.AddScoped<IProblemsApi, ProblemsApiStub>();
            services.AddScoped<IAccountApi, AccountApi>();
            services.AddScoped<IPermissionsApi, PermissionsApi>();
        }

        private static void AddDbContextPools(IServiceCollection services)
        {
            AddDbContextPool<User>(services, "users");
            AddDbContextPool<Token>(services, "tokens");
            AddDbContextPool<CollectionPermission>(services, "collection_permissions");
            AddDbContextPool<SubmitLimit>(services, "submit_limits");
        }

        private static void AddDbContextPool<TEntity>(IServiceCollection services, string dbName) where TEntity : class
        {
            services.AddScoped(provider => 
                new EntityContext<TEntity>(
                    new DbContextOptionsBuilder<EntityContext<TEntity>>()
                        .UseInMemoryDatabase(dbName)
                        .Options
                )
            );

            //TODO: Use this in next release of EntityFramework Core. See https://github.com/aspnet/EntityFrameworkCore/issues/9433
            //services.AddDbContextPool<EntityContext<TEntity>>(options => options.UseInMemoryDatabase(dbName));
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
    }
}