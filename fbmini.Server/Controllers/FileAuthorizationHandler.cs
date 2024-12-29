using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace fbmini.Server.Controllers
{
    public class FileAuthorizationHandler : AuthorizationHandler<FileAccessRequirement, FileModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FileAccessRequirement requirement, FileModel file)
        {
            string? id = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (id == file.OwnerId || file.AccessType == AccessType.Public)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class FileAccessRequirement : IAuthorizationRequirement { }

}
