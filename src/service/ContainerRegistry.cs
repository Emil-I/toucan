using StructureMap;
using Toucan.Contract;
using Toucan.Common;
using Toucan.Service.Security;

namespace Toucan.Service
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();

            For<ILocalAuthenticationService>().Use<LocalAuthenticationService>();
            For<IExternalAuthenticationService>().Use<ExternalAuthenticationService>();
            For<IExternalAuthenticationProvider>().Add<GoogleAuthenticationProvider>();
            For<IExternalAuthenticationProvider>().Add<MicrosoftAuthenticationProvider>();
            
            For<IAuditService>().Use<AuditService>();
            For<IManageUserService>().Use<ManageUserService>();
            For<IManageProfileService>().Use<ManageUserService>();
            For<ISecurityClaimsInspector>().Use<SecurityClaimsInspector>().Singleton();
            For<ISignupService>().Use<SignupService>();
            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
        }
    }
}