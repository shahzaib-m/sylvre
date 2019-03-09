using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sylvre.WebAPI.Swagger
{
    public class AuthHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            bool allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();

            if (!allowAnonymous && operation.OperationId == "Refresh")
            {
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = true,
                    Description = "The JWT refresh token to get a new access token and refresh token with as following: Bearer <jwt_token_here>",
                });
            }
            else if (!allowAnonymous && operation.OperationId == "Logout")
            {
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = true,
                    Description = "The JWT refresh token to log out and destroy as following: Bearer <jwt_token_here>",
                });
            }
            else if (!allowAnonymous)
            {
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = true,
                    Description = "The JWT access token to authenticate requests with as following: Bearer <jwt_token_here>",
                });
            }
        }
    }
}