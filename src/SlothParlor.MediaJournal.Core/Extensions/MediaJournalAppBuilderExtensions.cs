using Microsoft.AspNetCore.Builder;
using SlothParlor.MediaJournal.Core.Application;

namespace SlothParlor.MediaJournal.Core.Extensions;

public static class MediaJournalAppBuilderExtensions
{
    public static IApplicationBuilder UseMediaJournalUserData(this IApplicationBuilder app)
    {
        app.UseMiddleware<AppUserMiddleware>();

        return app;
    }
}
