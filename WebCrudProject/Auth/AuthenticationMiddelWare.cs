using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services.Interfaces;

namespace WebCrudProject.Auth
{
    public static class AuthenticationMiddelWare
    {
        public static string SessionID = "_=56qec56";

        public static async Task SessionResolve(HttpContext context, Func<Task> next)
        {
            var user = context.User;

            if (!IsAuthenticated(user))
            {
                var hasSession = TryGetSessionId(context, out var sessionId);

                if (!hasSession)
                {
                    await next();
                }

                var sessionService = context.RequestServices.GetRequiredService<ISessionService>();

                var session = await sessionService.ResolveSessionAsync(sessionId);

                if (session.Item1) // Check if valid session
                {
                    // Actual session
                    var _ses = session.Item2;

                    // Create claims
                    var claims = GetClaims(_ses);

                    // Create identity
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var princepal = new ClaimsPrincipal(identity);

                    // Set User
                    context.User = princepal;
                }
            }
            else
            {
                // Check expired
                var jsonExpiration = user.FindFirst(ClaimTypes.Expiration).Value;
                var sessionId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

                var expireDate = JsonSerializer.Deserialize<DateTime>(jsonExpiration);

                if (DateTime.Now > expireDate)
                {
                    // Rip, user time is up
                    context.Session.Set(SessionID, Array.Empty<byte>());
                    context.User = null;

                    var sessionService = context.RequestServices.GetRequiredService<ISessionService>();

                    await sessionService.DeleteSessionAsync(int.Parse(sessionId)).ConfigureAwait(false);
                }
            }

            await next();
        }

        private static IEnumerable<Claim> GetClaims(ELUJwtSession session)
        {
            var claims = new List<Claim>
            {
                CreateClaim(ClaimTypes.NameIdentifier, session.Id),
                CreateClaim(ClaimTypes.Email, session.Email),
                CreateClaim(ClaimTypes.Expiration, JsonSerializer.Serialize(session.ExpireDate)) // Date Format?
            };

            var roles = ELUser.GetRoles(session.CompressedRoles);

            foreach (var role in roles)
            {
                claims.Add(CreateClaim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private static Claim CreateClaim(string type, object value)
        {
            return new Claim(type, value.ToString() ?? "");
        }

        private static bool IsAuthenticated(ClaimsPrincipal user)
        {
            if (user.Identity == null)
                return false;

            return user.Identity.IsAuthenticated;
        }

        private static bool TryGetSessionId(HttpContext request, out string sessionId)
        {
            byte[]? byteArrayResult;

            var boolResult = request.Session.TryGetValue(SessionID, out byteArrayResult);

            if (boolResult 
                && byteArrayResult != null)
            {
                sessionId = Encoding.UTF8.GetString(byteArrayResult);
            }
            else
            {
                sessionId = string.Empty;
            }

            return boolResult;
        }
    }
}
