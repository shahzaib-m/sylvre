using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sylvre.WebAPI.Swagger;
using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services;
using Sylvre.WebAPI.Data.Enums;

namespace Sylvre.WebAPI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sylvre Web API",
                    Version = "v1",
                    Description = "An ASP.NET Core Web API for Sylvre.",
                });

                string coreDoc = Path.Combine(System.AppContext.BaseDirectory, "Sylvre.Core.xml");
                c.IncludeXmlComments(coreDoc, includeControllerXmlComments: true);

                string webApiDoc = Path.Combine(System.AppContext.BaseDirectory, "Sylvre.WebAPI.xml");
                c.IncludeXmlComments(webApiDoc, includeControllerXmlComments: true);

                c.AddSecurityDefinition("access-token", new OpenApiSecurityScheme
                {
                    Name = "Access token",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });
                c.AddSecurityDefinition("refresh-token", new OpenApiSecurityScheme
                {
                    Name = "Refresh token",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });

                c.OperationFilter<AuthenticationRequirementsFilter>();
                c.OperationFilter<ReauthenticationHeaderFilter>();
            });

            var connectionStr = $"User ID={Configuration["DbUser"]};" +
                                $"Password={Configuration["DbPassword"]};" +
                                $"Server={Configuration["DbServer"]};" +
                                $"Port={Configuration["DbPort"]};" +
                                $"Database={Configuration["DbName"]};" +
                                $"Pooling=true;";

            services.AddDbContext<SylvreWebApiContext>(opt => opt.UseNpgsql(connectionStr));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            var key = Encoding.ASCII.GetBytes(Configuration["AppSecret"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("AccessToken", options =>
            {
                ConfigureJwtBearerOptions(options, AuthTokenType.Access, key);
            })
            .AddJwtBearer("RefreshToken", options =>
            {
                ConfigureJwtBearerOptions(options, AuthTokenType.Refresh, key);
            });
        }

        private static void ConfigureJwtBearerOptions(JwtBearerOptions options, AuthTokenType tokenType, byte[] key)
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // attach access/refresh token to the context from the relevant cookie if they are using cookie auth strategy
                    switch (tokenType)
                    {
                        case AuthTokenType.Access:
                            if (context.Request.Cookies.ContainsKey("access-token"))
                                context.Token = context.Request.Cookies["access-token"];
                            break;
                        case AuthTokenType.Refresh:
                            if (context.Request.Cookies.ContainsKey("refresh-token"))
                                context.Token = context.Request.Cookies["refresh-token"];
                            break;
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    // client tried to reach an unauthenticated endpoint without the required token
                    context.Response.StatusCode = 401;
                    context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);
                    context.Response.Headers.Append(HeaderNames.ContentType, "application/json");
                    await context.Response.WriteAsync("{ \"message\": \"Unauthorized\" }");
                    context.HandleResponse();
                }
            };
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = "sylvre",
                ValidAudience = "sylvre-clients",
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // applying migrations at runtime
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<SylvreWebApiContext>()
                    .Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/documentation/{documentName}/docs.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.DocumentTitle = "Sylvre Web API Interactive Documentation";
                c.SwaggerEndpoint("/api/documentation/v1/docs.json", "Sylvre Web API V1");
                c.InjectStylesheet("/api/swagger-ui/theme-flattop.css");
            });

            if (!string.IsNullOrWhiteSpace(Configuration["TrustedProxies"]))
            {
                // use forwarded headers from specific trusted proxies specified in config to get real client IPs,
                // useful where IPs of clients are used (e.g. storing refresh token data to give users an overview of logged in sessions)
                var forwardedHeadersOpts = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                };
                foreach (string knownProxy in Configuration["TrustedProxies"].Split(','))
                {
                    if (IPAddress.TryParse(knownProxy, out IPAddress parsedAddress))
                    {
                        forwardedHeadersOpts.KnownProxies.Add(parsedAddress);
                    }
                }
                app.UseForwardedHeaders(forwardedHeadersOpts);
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
