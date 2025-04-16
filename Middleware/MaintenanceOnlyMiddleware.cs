namespace UserGuard_API.Middleware
{
    public class MaintenanceOnlyMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceOnlyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new GeneralResponse
                {
                    Success = false,
                    Data = "Unauthorized: User not authenticated"
                });
                return;
            }

            var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Maintenance")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new GeneralResponse
                {
                    Success = false,
                    Data = "Forbidden: Only maintenance staff can access this endpoint"
                });
                return;
            }

            await _next(context);
        }
    }
}
