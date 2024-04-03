using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SlothParlor.MediaJournal.Core.Application;

internal class AppUserMiddleware(RequestDelegate _next)
{
    public const string AppUserIdHttpItemsKey = "AppUserId";

    public async Task InvokeAsync(HttpContext context, ILogger<AppUserMiddleware> logger, IAppUserManager userService)
    {
        if (!context.User.Identity?.IsAuthenticated is true)
        {
            logger.LogDebug("Skipping user data middleware.");
            await _next(context);
            return;
        }
        
        var userProvider = context.RequestServices.GetRequiredService<IAppUserProvider>();

        userProvider.SetCurrentUser(await userService.GetOrCreateUserAsync(userProvider.CurrentUserId, context.User));

        await _next(context);
    }
}
