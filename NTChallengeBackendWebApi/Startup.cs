using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NTChallengeBackendWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTChallengeBackendWebApi
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<HackerNewsService, HackerNewsService>();

            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      //policy.WithOrigins("http://example.com",
                                      //                    "http://localhost:4200/");

                                      policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddApplicationInsightsTelemetry();
            // Configure the Connection String in appsettings.json

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IMemoryCache cache, IApplicationBuilder app, IWebHostEnvironment env, HackerNewsService hackerNewsService)
        {

            // This middleware serves generated Swagger document as a JSON endpoint
            app.UseSwagger();

            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NT Challenge Backend API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            _ = hackerNewsService.GetLatestStories().Result;
        }
    }
}
