namespace DailyTaskMakerAPI.Middleware
{
    using DailyTaskMaker.Infrastructure.Interfaces;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class RoleFetchingMiddleware
    {
        private readonly RequestDelegate _next;
        // private readonly IAuthenticationRepository _authRepo; // Assuming you have a service to fetch roles
        private readonly IServiceProvider _serviceProvider;

        public RoleFetchingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _authRepo = scope.ServiceProvider.GetRequiredService<IAuthenticationRepository>();
                if (context.User.Identity.IsAuthenticated)
                {
                    string userEmail = "";
                    var claimData = context.User.Claims.FirstOrDefault(x => x.Type == "preferred_username");
                    if (claimData != null)
                    {
                        userEmail = claimData.Value;
                    }
                    else
                    {
                        await _next(context);
                    }



                    // Fetch roles from the database
                    var roles = await _authRepo.FetchUserRolesFromDatabase(userEmail);

                    // Add roles as claims
                    var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));
                    var appIdentity = new ClaimsIdentity(claims);

                    context.User.AddIdentity(appIdentity);
                }
            }

            await _next(context);
        }
    }

   
    
}
