namespace WebCrudProject.Auth
{
    public static class UploadMiddelware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="next">Func<Task></param>
        /// <returns></returns>
        public static async Task UploadFilter(HttpContext context, Func<Task> next)
        {
            await next();
        }
    }
}
