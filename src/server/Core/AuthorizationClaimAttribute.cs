using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Service.Security;

namespace Toucan.Server.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizationClaimAttribute : AuthorizeAttribute
    {
        public const string PolicyName = "ClaimAttribute";

        public static Func<AuthorizationHandlerContext, bool> PolicyHandler = (context) =>
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                if (mvcContext.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor actionContext)
                {
                    var controller = actionContext.ControllerTypeInfo.GetCustomAttributes(true).OfType<AuthorizationClaimAttribute>();
                    var action = actionContext.MethodInfo.GetCustomAttributes(true).OfType<AuthorizationClaimAttribute>();
                    
                    var matches = controller.Union(action);
                    
                    var inspector = mvcContext.HttpContext.RequestServices.GetService<ISecurityClaimsInspector>();

                    return matches.All(o => inspector.Satisifies(context.User, o.RequirementType, o.ClaimType, o.Values));
                }
            };

            return false;
        };

        public AuthorizationClaimAttribute(ClaimRequirementType requirementType, string claimType, params object[] values) : base(PolicyName)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentNullException(nameof(claimType));

            ClaimType = claimType;
            RequirementType = requirementType;
            Values = values;
        }

        public string ClaimType { get; }
        public ClaimRequirementType RequirementType { get; }
        public object[] Values { get; }
    }
}