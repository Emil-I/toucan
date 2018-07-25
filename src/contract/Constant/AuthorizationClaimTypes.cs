namespace Toucan.Contract.Security
{
    public static class AuthorizationClaimTypes
    {
        public const string AllowedValuesPattern = @"^([\*]|([CcRrUuDd]{1,}))$";
        public const string CustomClaim = "custom";
    }
}