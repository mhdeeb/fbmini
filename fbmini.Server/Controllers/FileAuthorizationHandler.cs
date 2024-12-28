using fbmini.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace fbmini.Server.Controllers
{
    public class FileAuthorizationHandler : AuthorizationHandler<FileAccessRequirement, FileModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FileAccessRequirement requirement, FileModel file)
        {
            if (context.User.Identity?.Name == file.OwnerId || file.AccessType == AccessType.Public)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class FileAccessRequirement : IAuthorizationRequirement { }

}
