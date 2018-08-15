
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Toucan.Contract;
using Toucan.Server.Core;

namespace Toucan.Server.Formatters
{
    public class DateTimeOutputFormatterJson : JsonOutputFormatter
    {
        public DateTimeOutputFormatterJson(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool) : base(serializerSettings, charPool)
        {
        }
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var resolver = context.HttpContext.RequestServices.GetService<IHttpServiceContextResolver>();

            IDomainContext domainContext = resolver.Resolve();

            var settings = new JsonSerializerSettings()
            {
                Culture = domainContext.Culture,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            settings.Converters.Add(new DateTimeConverter(domainContext.SourceTimeZone));

            OutputFormatterWriteContext alt = null;

            using (var ms = new MemoryStream())
            {
                using (var jw = new JsonTextWriter(new StreamWriter(ms)))
                {
                    var s = JsonSerializer.Create(settings);
                    s.Serialize(jw, context.Object);
                    jw.Flush();
                    ms.Position = 0;
                    var r = new JsonTextReader(new StreamReader(ms));
                    var copy = s.Deserialize(r, context.ObjectType);
                    alt = new OutputFormatterWriteContext(context.HttpContext, context.WriterFactory, context.ObjectType, copy);
                }
            };

            return base.WriteResponseBodyAsync(alt ?? context, selectedEncoding);
        }
    }
}