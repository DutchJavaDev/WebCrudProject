using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using WebCrudProject.Models;

namespace WebCrudProject.Auth
{
    public static class SignInMiddelware
    {

        public static readonly string SignIngCookie = "sgnCku343";

        /// <summary>
        /// Checks if there is a signin cookie, if so redirect to previous url or else to /, else send to register & login
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="next">Func<Task></param>
        /// <returns></returns>
        public static async Task CheckSignIn(HttpContext context, Func<Task> next)
        {
        }
    }
}
