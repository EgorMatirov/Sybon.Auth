using System;
using System.Collections.Generic;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private const string ServiceName = "Sybon.Auth";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SecurityConfiguration = new AuthSecurityConfiguration(configuration.GetSection("Security"));
        }

        private AuthSecurityConfiguration SecurityConfiguration { get; }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            var metrics = AppMetrics.CreateDefaultBuilder()
                .Report.ToInfluxDb(options =>
                {
                    options.InfluxDb.Password = SecurityConfiguration.InfluxDb.Password;
                    options.InfluxDb.UserName = SecurityConfiguration.InfluxDb.UserName;
                    options.InfluxDb.BaseUri = new Uri(SecurityConfiguration.InfluxDb.Url);
                    options.InfluxDb.Database = SecurityConfiguration.InfluxDb.Database;
                    options.FlushInterval = TimeSpan.FromSeconds(1);
                })
                .Configuration.ReadFrom(Configuration)
                .Configuration.Configure(
                    options =>
                    {
                        options.AddAppTag(ServiceName);
                        options.AddEnvTag("development");
                    })
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsReportScheduler();
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            
            
            services.AddMvc(options => options.AddMetricsResourceFilter());

            services.AddSwagger(ServiceName, "v1");
            
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
            
            services.AddScoped<IAccountApi, AccountApi>();
            services.AddScoped<IPermissionsApi, PermissionsApi>();
            services.AddSingleton<IProblemsApi>(new ProblemsApi(new Configuration
            {
                BasePath = SecurityConfiguration.SybonArchive.Url,
                ApiKey = new Dictionary<string, string>
                {
                    {"api_key", SecurityConfiguration.ApiKey}
                }
            }));

            ConfigureMapper();
            services.AddSingleton(Mapper.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseMetricsAllMiddleware();
            app.UseMetricsAllEndpoints();
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