
using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Service.Security
{
    public interface ISecurityClaimsInspector
    {
        bool Satisifies(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values);
        Task<bool> SatisifiesAsync(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values);
    }
}