using System.IO;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

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

namespace Sylvre.WebAPI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
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

                c.OperationFilter<AuthHeaderFilter>();
                c.OperationFilter<ReauthenticationHeaderFilter>();
            });

            var connectionStr = $"User ID={Configuration["DbUser"]};" +
                                $"Password={Configuration["DbPassword"]};" +
                                $"Server={Configuration["DbServer"]};" +
                                $"Port={Configuration["DbPort"]};" +
                                $"Database={Configuration["DbName"]};" +
                                $"Integrated Security=true;" +
                                $"Pooling=true;";

            services.AddDbContext<SylvreWebApiContext>(opt => opt.UseNpgsql(connectionStr));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            var key = Encoding.ASCII.GetBytes(Configuration["AppSecret"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("AccessToken", options =>
            {
                options.Events = new JwtBearerEvents
                {
                    // custom response for missing token/cookie instead of empty response body
                    // https://stackoverflow.com/questions/38281116/custom-401-and-403-response-model-with-usejwtbearerauthentication-middleware
                    OnChallenge = async context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                            context.Options.Challenge);

                        context.Response.Headers.Append(HeaderNames.ContentType,
                            "application/json");

                        await context.Response.WriteAsync("{ \"message\": \"Unauthorized\" }");

                        context.HandleResponse();
                    },
                    OnMessageReceived = context =>
                    {
                        // if the user is authenticating via a cookie (browsers/web-apps), use that token
                        if (context.Request.Cookies.ContainsKey("access-token"))
                            context.Token = context.Request.Cookies["access-token"];

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var userEntity = await userService.RetrieveAsync(userId);
                        if (userEntity == null)
                        {
                            context.Fail("Unauthorized");
                        }

                        return;
                    }
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = "sylvre-webapi",
                    ValidAudience = "sylvre-webapi-client-access-token"
                };
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("RefreshToken", options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        // custom response for missing token/cookie instead of empty response body
                        // https://stackoverflow.com/questions/38281116/custom-401-and-403-response-model-with-usejwtbearerauthentication-middleware
                        OnChallenge = async context =>
                        {
                            context.Response.StatusCode = 401;
                            context.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                                context.Options.Challenge);

                            context.Response.Headers.Append(HeaderNames.ContentType,
                                "application/json");

                            await context.Response.WriteAsync("{ \"message\": \"Unauthorized\" }");

                            context.HandleResponse();
                        },
                        OnMessageReceived = context =>
                        {
                            // if the user is authenticating via a cookie (browsers/web-apps), use that token
                            if (context.Request.Cookies.ContainsKey("refresh-token"))
                                context.Token = context.Request.Cookies["refresh-token"];

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var userId = int.Parse(context.Principal.Identity.Name);
                            var userEntity = await userService.RetrieveAsync(userId);
                            if (userEntity == null)
                            {
                                context.Fail("Unauthorized");
                            }

                            // adding the raw signature of the refresh token for use in the /auth/refresh action
                            var refreshToken = context.SecurityToken as JwtSecurityToken;
                            if (refreshToken != null)
                            {
                                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                                if (identity != null)
                                {
                                    identity.AddClaim(new Claim("refresh-token-signature", refreshToken.RawSignature));
                                }
                            }

                            return;
                        }
                    };
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuer = "sylvre-webapi",
                        ValidAudience = "sylvre-webapi-client-refresh-token"
                    };
                });
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

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
