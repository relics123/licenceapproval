using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Helpers
{
    public static class SessionHelper
    {
        private const string TokenKey = "JwtToken";
        private const string UserKey = "UserInfo";

        public static void SetToken(ISession session, string token)
        {
            session.SetString(TokenKey, token);
        }

        public static string GetToken(ISession session)
        {
            return session.GetString(TokenKey);
        }

        public static void SetUserInfo(ISession session, UserInfo user)
        {
            var json = JsonSerializer.Serialize(user);
            session.SetString(UserKey, json);
        }

        public static UserInfo GetUserInfo(ISession session)
        {
            var json = session.GetString(UserKey);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<UserInfo>(json);
        }

        public static void ClearSession(ISession session)
        {
            session.Clear();
        }

        public static bool IsAuthenticated(ISession session)
        {
            return !string.IsNullOrEmpty(GetToken(session));
        }
    }
}