using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tripod.Web
{
    public class CamelCaseJsonResult : JsonResult
    {
        private readonly JsonSerializerSettings _settings;

        public CamelCaseJsonResult(object data, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet, Encoding contentEncoding = null, string contentType = null)
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            Data = data;
            JsonRequestBehavior = behavior;
            ContentEncoding = contentEncoding;
            ContentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data == null) return;
            var serializer = JsonSerializer.Create(_settings);
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, Data);
                response.Write(sw.ToString());
            }
        }
    }
}