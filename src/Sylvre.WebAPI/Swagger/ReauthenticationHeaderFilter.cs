using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sylvre.WebAPI.Swagger
{
    public class ReauthenticationHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            bool allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();

            if (!allowAnonymous && 
               ((operation.OperationId == "DeleteUser" ) || (operation.OperationId == "ChangePassword")))
            {
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Sylvre-Reauthenticate-Pass",
                    In = "header",
                    Type = "string",
                    Required = true,
                    Description = "The base64 encoded password of the authenticated user/identity (password re-authentication for unsafe endpoints).",
                });
            }
        }
    }
}