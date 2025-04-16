// Middleware/AdminMiddleware.cs
public class AdminMiddleware
{
    private readonly RequestDelegate _next;

    public AdminMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
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
        var user = await userManager.GetUserAsync(context.User);

        // إذا كان اليوزر أدمن، استمر في الـ Request
        if (user != null && await userManager.IsInRoleAsync(user, "Admin"))
        {
            await _next(context);
        }
        else
        {
          
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new GeneralResponse { Success = false, Data = "You do not have permission to access this resource." });
            return;
        }
    }
}
