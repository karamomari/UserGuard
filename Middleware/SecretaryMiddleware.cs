namespace UserGuard_API.Middleware
{
    public class SecretaryOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new
                {
                    Success = false,
                    Data = "Unauthorized: User not authenticated"
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Secretary")
            {
                context.Result = new JsonResult(new
                {
                    Success = false,
                    Data = "Forbidden: Only secretaries can access this endpoint"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }

}
