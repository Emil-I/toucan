using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Toucan.Contract.Security;
using Microsoft.Extensions.Options;

namespace Toucan.Service.Security
{
    public class SecurityClaimsInspector : ISecurityClaimsInspector
    {
        private static Dictionary<ClaimRequirementType, Func<Claim, object[], bool>> strategies = new Dictionary<ClaimRequirementType, Func<Claim, object[], bool>>()
        {
            { ClaimRequirementType.Any, StrategyAny},
            { ClaimRequirementType.Exists, StrategyExists},
            { ClaimRequirementType.RegexPattern, StrategyRegexPattern},
            { ClaimRequirementType.Strict, StrategyStrict}
        };
        private readonly Config config;

        public SecurityClaimsInspector(IOptions<Service.Config> config)
        {
            this.config = config.Value;
        }

        public bool Satisifies(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values)
        {
            string claimsKey = this.config.ClaimsNamespace + claimType;

            Claim claim = principal.Claims.FirstOrDefault(o => o.Type == claimsKey);

            Func<Claim, object[], bool> strategy = strategies.Single(o => o.Key == requirementType).Value;

            return strategy(claim, values);
        }

        public Task<bool> SatisifiesAsync(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claim, params object[] values)
        {
            return Task.FromResult(this.Satisifies(principal, requirementType, claim, values));
        }

        private static bool StrategyAny(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            if (claim.Value == AuthorizationClaimValueTypes.Any)
                return true;

            if (values.Length == 0)
                return true;

            return values.Where(o => o != null).Any(o => String.Equals(o.ToString(), claim.Value));
        }

        private static bool StrategyExists(Claim claim, params object[] values)
        {
            return claim != null;
        }

        private static bool StrategyRegexPattern(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            string pattern = values.Length > 0 ? values[0].ToString() : null;

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentOutOfRangeException($"Authorization attribute is incorrectly configured. Expected the first parameter to be a valid regex pattern");

            try
            {
                return Regex.IsMatch(claim.Value, pattern);
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException($"Authorization attribute is incorrectly configured. A regular expression parsing error occurred");
            }
        }

        private static bool StrategyStrict(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            if (claim.Value == AuthorizationClaimValueTypes.Any)
                return true;

            if (values.Length == 0)
                return true;

            return values.Where(o => o != null).All(o => String.Equals(o.ToString(), claim.Value));
        }
    }
}