using System.Linq;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sylvre.WebAPI.Swagger
{
    public class ReauthenticationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            bool allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();

            if (!allowAnonymous && 
               ((operation.OperationId == "DeleteUser" ) || (operation.OperationId == "ChangePassword")))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Sylvre-Reauthenticate-Pass",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "The base64 encoded password of the authenticated user/identity (password re-authentication for unsafe endpoints).",
                });
            }
        }
    }
}