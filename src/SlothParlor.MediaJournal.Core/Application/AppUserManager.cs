using Microsoft.Extensions.Logging;
using SlothParlor.MediaJournal.Core.Extensions;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;
using System.Security.Claims;

namespace SlothParlor.MediaJournal.Core.Application;

public class AppUserManager(ILogger<AppUserManager> _logger, MediaJournalDbContext _dbContext) : IAppUserManager
{
    public async Task<User> GetOrCreateUserAsync(string userId, ClaimsPrincipal principal)
    {
        var existingUser = await _dbContext.Users.FindAsync(userId);
        
        if (existingUser is not null)
        {
            return existingUser;
        }

        var user = new User()
        {
            UserId = userId,
            Email = principal.GetPrimaryEmail(),
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        using var userScope = _logger.BeginScope(user);

        _logger.LogInformation("User created.");

        return user;
    }
}
