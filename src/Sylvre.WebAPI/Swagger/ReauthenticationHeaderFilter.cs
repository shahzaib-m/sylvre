using System.Linq;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.SwaggerGen;

using Sylvre.WebAPI.Controllers;

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
            var unsafeOperationNames = new List<string>
            {
                nameof(UsersController.DeleteUser),
                nameof(UsersController.ChangePassword)
            };
            if (!allowAnonymous && unsafeOperationNames.Contains(operation.OperationId))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Sylvre-Reauthenticate-Pass",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "The base64 encoded password of the authenticated user/identity (unsafe endpoints require password re-authentication for non-admins).",
                });
            }
        }
    }
}