using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlothParlor.MediaJournal.Core.JournalUser;
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

        serviceCollection.AddScoped<IUserService, UserService>();

        return serviceCollection;
    }
}
