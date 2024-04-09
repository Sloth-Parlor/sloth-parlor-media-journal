using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlothParlor.MediaJournal.Core.Application;
using SlothParlor.MediaJournal.Core.Journal;
using SlothParlor.MediaJournal.Core.Mapping;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.WebApp;

namespace SlothParlor.MediaJournal.Core.Extensions;

public static class MediaJournalServiceCollectionExtensions
{
    public static IServiceCollection AddMediaJournalCore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<MediaJournalDbContext>((services, options) =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var hostEnvironment = services.GetService<IHostEnvironment>();
            
            var appDbConnectionString = configuration.GetConnectionString(Constants.AppDbConnectionStringKey);
            options.UseNpgsql(appDbConnectionString);

            if (hostEnvironment is not null && hostEnvironment.IsDevelopment())
            {
                options
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            }
        });

        serviceCollection.AddAutoMapper(mapperConfiguration =>
        {
            mapperConfiguration.AddProfile<WatchGroupProfile>();
            mapperConfiguration.AddProfile<WatchGroupParticipantProfile>();
        });

        serviceCollection.AddScoped<IAppUserProvider, AppUserProvider>();
        serviceCollection.AddScoped<IAppUserManager, AppUserManager>();
        serviceCollection.AddScoped<IWatchGroupManager, WatchGroupManager>();
        serviceCollection.AddScoped<IUserJournalRepository, UserJournalRepository>((services) => 
        {
            var userProvider = services.GetRequiredService<IAppUserProvider>();

            return ActivatorUtilities.CreateInstance<UserJournalRepository>(
                services, userProvider.CurrentUserId);
        });

        return serviceCollection;
    }
}
