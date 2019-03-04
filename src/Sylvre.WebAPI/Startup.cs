using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.Swagger;

using Sylvre.WebAPI.Models;

namespace Sylvre.WebAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.${env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                             .AddJsonOptions(opt =>
                             {
                                 opt.SerializerSettings.Converters.Add(
                                     new Newtonsoft.Json.Converters.StringEnumConverter());
                                 opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                             });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Sylvre Web API",
                    Version = "v1",
                    Description = "An ASP.NET Core Web API for Sylvre.",
                });

                string coreDoc = Path.Combine(System.AppContext.BaseDirectory, "Sylvre.Core.xml");
                c.IncludeXmlComments(coreDoc, includeControllerXmlComments: true);

                string webApiDoc = Path.Combine(System.AppContext.BaseDirectory, "Sylvre.WebAPI.xml");
                c.IncludeXmlComments(webApiDoc, includeControllerXmlComments: true);
            });

            services.AddEntityFrameworkNpgsql().AddDbContext<SylvreWebApiContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("SylvreWebApiDbConnection")));

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "documentation/{documentName}/docs.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "documentation";
                c.DocumentTitle = "Sylvre Web API Interactive Documentation";
                c.SwaggerEndpoint("/documentation/v1/docs.json", "Sylvre Web API V1");
            });

            app.UseCors(opt => 
                opt.WithOrigins(Configuration["AllowedOrigins"])
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            app.UseMvc();
        }
    }
}
