using System.IO;
using System.Net;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.Swagger;

using Sylvre.WebAPI.Swagger;
using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services;

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

                c.OperationFilter<AuthHeaderFilter>();
                c.OperationFilter<ReauthenticationHeaderFilter>();
            });

            services.AddEntityFrameworkNpgsql().AddDbContext<SylvreWebApiContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("SylvreWebApiDbConnection")));

            services.AddCors();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();


            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // applying migrations at runtime
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<SylvreWebApiContext>()
                    .Database.Migrate();
            }
            //

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            string apiDocsRootUrl = Configuration.GetSection("AppSettings").Get<AppSettings>().ApiDocsRootUrl;

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Sylvre Web API Interactive Documentation";
                c.SwaggerEndpoint("./v1/docs.json", "Sylvre Web API V1");
            });

            string corsOrigin = Configuration.GetSection("AppSettings").Get<AppSettings>().CorsOrigin;
            app.UseCors(opt =>
                 opt.WithOrigins(corsOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

            // use forwarded headers from reverse proxies, accept from all private IP addresses and known exact proxies
            var forwardedHeadersOpts = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOpts.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
            forwardedHeadersOpts.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
            forwardedHeadersOpts.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("192.168.0.0"), 16));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            foreach (string knownProxy in appSettings.KnownProxies.Split(','))
            {
                if (IPAddress.TryParse(knownProxy, out IPAddress parsedAddress))
                {
                    forwardedHeadersOpts.KnownProxies.Add(parsedAddress);
                }
            }
            app.UseForwardedHeaders(forwardedHeadersOpts);
            //

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
