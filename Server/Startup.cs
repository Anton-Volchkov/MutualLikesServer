using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MutualLikes.Application.VK;
using MutualLikes.DataAccess;
using Server.HostedServices;
using Server.Hubs;
using VkNet;
using VkNet.Abstractions;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;

namespace Server
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var builder = new ConfigurationBuilder()
                          .SetBasePath(env.ContentRootPath)
                          .AddJsonFile("appsettings.json", false, true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                          .AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                                                            options.UseNpgsql(connectionString));
            services.AddHostedService<MigrationHostedService>();

            services.AddSignalR();

            services.AddControllers();


            services.AddSingleton<IVkApi>(sp =>
            {
                services.AddAudioBypass();
                var api = new VkApi(services);
                api.Authorize(new ApiAuthParams { AccessToken = Configuration["Config:AccessToken"] });
                api.RequestsPerSecond = 10;
                return api;
            });

            services.AddSingleton<VkFinder>();

            services.AddMediatR(typeof(VkFinder).GetTypeInfo().Assembly);


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                                  builder => builder.WithOrigins("https://anton-volchkov.github.io")
                                                    .AllowAnyMethod()
                                                    .AllowAnyHeader());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
              
                endpoints.MapControllers();
                endpoints.MapHub<ServerHub>("/server");

            });


        }
    }
}
