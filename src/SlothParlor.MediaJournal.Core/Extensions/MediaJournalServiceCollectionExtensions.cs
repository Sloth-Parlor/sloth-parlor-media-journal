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

        serviceCollection.AddAutoMapper(mapperConfig =>
        {
            mapperConfig.AddDefaultProfiles();
        });

        serviceCollection.AddScoped<IAppUserProvider, AppUserProvider>();
        serviceCollection.AddScoped<IAppUserManager, AppUserManager>();
        serviceCollection.AddScoped<IWatchGroupRepositoryFactory, WatchGroupRepositoryFactory>();
        serviceCollection.AddScoped<IMediaLogRepositoryFactory, MediaLogRepositoryFactory>();
        serviceCollection.AddScoped((serviceProvider) =>
        {
            var appUserProvider = serviceProvider.GetRequiredService<IAppUserProvider>();
            var factory = serviceProvider.GetRequiredService<IWatchGroupRepositoryFactory>();

            return factory.Create(appUserProvider.CurrentUserId);
        });

        return serviceCollection;
    }

    public static IServiceCollection AddMediaJournalDb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediaJournalDb((services, options) =>
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

        return serviceCollection;
    }

    public static IServiceCollection AddMediaJournalDb(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddMediaJournalDb((_, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        return serviceCollection;
    }

    public static IServiceCollection AddMediaJournalDb(this IServiceCollection serviceCollection, Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction)
    {
        serviceCollection.AddDbContext<MediaJournalDbContext>(optionsAction);

        return serviceCollection;
    }
}
