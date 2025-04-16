namespace UserGuard_API.Middleware
{
    public class SecretaryOnlyMiddleware
    {
        private readonly RequestDelegate _next;

        public SecretaryOnlyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // تحقق من وجود المستخدم
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { Success = false, Data = "Unauthorized: User not authenticated" });
                return;
            }

            // استخرج الـ Role من الـ Claims
            var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Secretary")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new GeneralResponse{ Success = false, Data = "Forbidden: Only secretaries can access this endpoint" });
                return;
            }

            // إذا كل شيء تمام، كمّل المسار
            await _next(context);
        }
    }

}
