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


var builder = WebApplication.CreateBuilder(args);

// Configure the configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Register services and configure the application
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

var connectionStr = $"User ID={builder.Configuration["DbUser"]};" +
                    $"Password={builder.Configuration["DbPassword"]};" +
                    $"Server={builder.Configuration["DbServer"]};" +
                    $"Port={builder.Configuration["DbPort"]};" +
                    $"Database={builder.Configuration["DbName"]};" +
                    $"Pooling=true;";

builder.Services.AddDbContext<SylvreWebApiContext>(opt => opt.UseNpgsql(connectionStr));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["AppSecret"]);
builder.Services.AddAuthentication(options =>
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

var app = builder.Build();

// applying migrations at runtime
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<SylvreWebApiContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
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
    c.SwaggerEndpoint("documentation/v1/docs.json", "Sylvre Web API V1");

    c.InjectStylesheet("swagger-ui-themes/modern.common.css");
    c.InjectStylesheet("swagger-ui-themes/modern.dark.css");
    c.InjectJavascript("swagger-ui-themes/modern.js");
});

if (!string.IsNullOrWhiteSpace(builder.Configuration["TrustedProxies"]))
{
    // use forwarded headers from specific trusted proxies specified in config to get real client IPs,
    // useful where IPs of clients are used (e.g. storing refresh token data to give users an overview of logged in sessions)
    var forwardedHeadersOpts = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    };
    foreach (string knownProxy in builder.Configuration["TrustedProxies"].Split(','))
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
app.MapControllers();

app.Run();

static void ConfigureJwtBearerOptions(JwtBearerOptions options, AuthTokenType tokenType, byte[] key)
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
