using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Server.Model;
using Toucan.Server.Security;
using Toucan.Service.Security;

namespace Toucan.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ContentController : ControllerBase
    {
        public ContentController(IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
        }

        [HttpGet()]
        [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
        public async Task<object> RikerIpsum([FromQuery]DateTime clientTime)
        {
            DateTime roundTripTime = TimeZoneInfo.ConvertTimeFromUtc(clientTime, this.DomainContext.SourceTimeZone);

            var payload = new Model.Payload<object>()
            {
                Data = !this.DomainContext.User.Enabled ? this.Dictionary["home.body.0"].Value : this.Dictionary["home.body.1"].Value,
                Message = new PayloadMessage()
                {
                    MessageType = PayloadMessageType.Info,
                    Text = string.Format(this.Dictionary["home.text"].Value, roundTripTime.ToString("hh:mm tt"))
                }
            };

            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1 * 1000);
                return payload;
            });
        }

        [HttpGet]
        [AuthorizationClaim(ClaimRequirementType.Strict, AuthorizationClaimTypes.CustomClaim, AuthorizationClaimValueTypes.Read)]
        public async Task<object> SecureUserContent()
        {
            var payload = new Model.Payload<string>()
            {
                Data = "=] a secret smile for you!",
                Message = new PayloadMessage()
                {
                    MessageType = PayloadMessageType.Success
                }
            };

            return await Task.FromResult(payload);
        }
    }
}
