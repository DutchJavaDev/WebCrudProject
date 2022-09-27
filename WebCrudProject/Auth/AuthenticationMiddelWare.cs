namespace WebCrudProject.Auth
{
    public static class AuthenticationMiddelWare
    {
        public static async Task SessionResolve(HttpContext context, Func<Task> next)
        {
            await next();
        }

        public static async Task CookieResolve(HttpContext context, Func<Task> next)
        {
            await next();
        }
    }
}
