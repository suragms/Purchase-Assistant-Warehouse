using Microsoft.AspNetCore.Authorization;
using PurchaseAssistant.Domain.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            var permissions = context.User.FindAll("permissions").Select(c => c.Value);
            var role = context.User.FindFirst("role")?.Value;

            // In our system, Owner has implicit omnipotence over their tenant.
            if (role == "Owner" || role == "SuperAdmin" || permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}