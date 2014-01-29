using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

            var cookie = new HttpCookie(CookieName, "")
            {
                Expires = DateTime.UtcNow.AddDays(-2),
            };

            int userIdInt;
            if (int.TryParse(userId, out userIdInt))
            {
                var data = queries.Execute(new ClientCookieBy(userIdInt)).Result;
                var json = JsonConvert.SerializeObject(data);
                var cookieTokens = DependencyResolver.Current.GetService<IProvideTokenizers>().CookieEncryptionTokens;
                var userToken = new UserToken
                {
                    CreationDate = DateTime.UtcNow,
                    Value = json,
                    UserId = userId,
                };
                var cookieValue = cookieTokens.Generate(userToken);
                cookie.Values[ClientCookieKey] = cookieValue;
                cookie.Expires = DateTime.UtcNow.AddDays(60);
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
                var protectedValue = cookie.Values[ClientCookieKey];
                var cookieTokens = DependencyResolver.Current.GetService<IProvideTokenizers>().CookieEncryptionTokens;
                var userToken = cookieTokens.Validate(protectedValue);
                if (userToken != null)
                {
                    json = userToken.Value;
                }
            }
            
            var clientCookie = JsonConvert.DeserializeObject<ClientCookie>(json);
            return clientCookie;
        }
    }
}
