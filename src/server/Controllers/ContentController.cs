using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Toucan.Contract;
using Toucan.Server.Model;

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
        public async Task<object> RikerIpsum([FromQuery]DateTime @default, [FromQuery]DateTime locale, [FromQuery]DateTime iso, [FromQuery]DateTime utc)
        {
            Model.Payload<object> payload = null;
            IEqualityComparer<DateTime> comparer = new AccurateToSeconds();

            var times = new DateTime[]
            {
                TimeZoneInfo.ConvertTimeFromUtc(@default, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(locale, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(iso, this.DomainContext.SourceTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(utc, this.DomainContext.SourceTimeZone)
            };

            var delta = times.Distinct(comparer).ToList();

            if (delta.Count > 1)
            {
                string expected = times.First().ToString("hh:mm tt");
                string text = $"Expected all dates to be '{expected}', but {(delta.Count - 1)} dates did not match";

                payload = new Model.Payload<object>()
                {
                    Data = !this.DomainContext.User.Enabled ? this.Dictionary["home.body.0"].Value : this.Dictionary["home.body.1"].Value,
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Error,
                        Text = text
                    }
                };
            }
            else
            {
                DateTime roundTripTime = TimeZoneInfo.ConvertTimeFromUtc(@default, this.DomainContext.SourceTimeZone);

                payload = new Model.Payload<object>()
                {
                    Data = !this.DomainContext.User.Enabled ? this.Dictionary["home.body.0"].Value : this.Dictionary["home.body.1"].Value,
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Info,
                        Text = string.Format(this.Dictionary["home.text"].Value, roundTripTime.ToString("hh:mm tt"))
                    }
                };
            }

            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1 * 1000);
                return payload;
            });
        }

        private class AccurateToSeconds : IEqualityComparer<DateTime>
        {
            public bool Equals(DateTime x, DateTime y)
            {
                return this.GetHashCode(x) == this.GetHashCode(y);
            }

            public int GetHashCode(DateTime obj)
            {
                var timeOfDay = new TimeSpan(obj.TimeOfDay.Hours, obj.TimeOfDay.Minutes, obj.TimeOfDay.Seconds);
                return obj.Date.ToString().GetHashCode() + timeOfDay.GetHashCode();
            }
        }
    }
}
