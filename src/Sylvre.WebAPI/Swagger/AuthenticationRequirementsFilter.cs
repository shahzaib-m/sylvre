using System.Linq;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.SwaggerGen;

using Sylvre.WebAPI.Controllers;

namespace Sylvre.WebAPI.Swagger
{
    public class AuthenticationRequirementsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();

            bool allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();
            var refreshTokenOperationNames = new List<string>
            {
                nameof(AuthController.Refresh),
                nameof(AuthController.Logout)
            };
            if (!allowAnonymous && refreshTokenOperationNames.Contains(operation.OperationId))
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "refresh-token"
                    }
                };

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = []
                });
            }
            else if (!allowAnonymous)
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "access-token"
                    }
                };

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = []
                });
            }
        }
    }
}
