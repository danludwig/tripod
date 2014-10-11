using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using Tripod.Domain.Security;

namespace Tripod.Web
{
    public static class CookieExtensions
    {
        private const string CookieName = "Tripod.ApplicationCookie";
        private const string ClientCookieKey = "g";

        public static void ClientCookie(this HttpResponseBase response, int userId, IProcessQueries queries)
        {
            response.ClientCookie(userId.ToString(CultureInfo.InvariantCulture), queries);
        }

        public static void ClientCookie(this HttpResponseBase response, string userId, IProcessQueries queries)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (queries == null) throw new ArgumentNullException("queries");

            var cookie = new HttpCookie(CookieName, "");

            int userIdInt;
            if (int.TryParse(userId, out userIdInt))
            {
                var data = queries.Execute(new ClientCookieBy(userIdInt)).Result;
                var json = JsonConvert.SerializeObject(data);
                byte[] jsonBytes = MachineKey.Protect(Encoding.UTF8.GetBytes(json), "Cookie");
                string cookieValue = HttpServerUtility.UrlTokenEncode(jsonBytes);
                cookie.Values[ClientCookieKey] = cookieValue;
                //cookie.Expires = DateTime.UtcNow.AddDays(60);
            }
            else
            {
                cookie.Expires = DateTime.UtcNow.AddDays(-2);
            }

            response.SetCookie(cookie);
        }

        public static ClientCookie ClientCookie(this HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var json = "{}";
            var cookie = request.Cookies.Get(CookieName);
            if (cookie != null)
            {
                var cookieValue = cookie.Values[ClientCookieKey];
                var jsonBytes = HttpServerUtility.UrlTokenDecode(cookieValue);
                if (jsonBytes != null)
                    jsonBytes = MachineKey.Unprotect(jsonBytes, "Cookie");
                if (jsonBytes != null)
                    json = Encoding.UTF8.GetString(jsonBytes);
            }

            var clientCookie = JsonConvert.DeserializeObject<ClientCookie>(json);
            return clientCookie;
        }
    }
}
