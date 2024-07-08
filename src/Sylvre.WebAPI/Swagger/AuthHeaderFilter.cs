using System.Linq;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sylvre.WebAPI.Swagger
{
    public class AuthHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            bool allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();

            if (!allowAnonymous && operation.OperationId == "Refresh")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "The refresh token (JWT) to get a new access token and refresh token with as following: Bearer <jwt_here>",
                });
            }
            else if (!allowAnonymous && operation.OperationId == "Logout")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "The refresh token (JWT) to log out and destroy as following: Bearer <jwt_here>",
                });
            }
            else if (!allowAnonymous)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "The access token (JWT) to authenticate requests with as following: Bearer <jwt_here>",
                });
            }
        }
    }
}