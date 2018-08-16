using System;
using System.Linq;
using Toucan.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        private static string MediaTypeJson = "application/json";
        public static bool AcceptsJsonResponse(this HttpRequest request)
        {
            var contentTypes = request.GetTypedHeaders().Accept;

            return contentTypes.Any(o => o.MediaType == MediaTypeJson || o.MediaType.Value.ToLower().Contains("json"));
        }

        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null, Stream inputStream = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (inputStream == null)
                inputStream = request.Body;

            using (StreamReader reader = new StreamReader(inputStream, encoding))
                return await reader.ReadToEndAsync();
        }

        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request, Stream inputStream = null)
        {
            if (inputStream == null)
                inputStream = request.Body;

            using (var ms = new MemoryStream(2048))
            {
                await inputStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}