using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SlothParlor.MediaJournal.Core.JournalUser;

internal class UserDataMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<UserDataMiddleware> logger, IUserService userService)
    {
        if (!context.User.Identity?.IsAuthenticated is true)
        {
            logger.LogDebug("Skipping user data middleware.");
            await _next(context);
            return;
        }

        await userService.CreateUserIfNotExistsAsync(context.User);
        await _next(context);
    }
}
