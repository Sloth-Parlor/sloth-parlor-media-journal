using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SlothParlor.MediaJournal.Data.Models;
using System.Security.Claims;

namespace SlothParlor.MediaJournal.Core.JournalUser;

public class UserService(ILogger<UserService> _logger, MediaJournalDbContext _dbContext) : IUserService
{
    public async Task CreateUserIfNotExistsAsync(ClaimsPrincipal principal)
    {
        var user = new User()
        {
            ObjectId = GetRequiredClaim(principal, Authentication.UserClaimTypes.ObjectId),
            Provider = GetRequiredClaim(principal, Authentication.UserClaimTypes.Idp),
            Email = GetRequiredClaim(principal, Authentication.UserClaimTypes.Emails),
        };

        using var userEntityBeforeCreateScope = _logger.BeginScope(new Dictionary<string, object?>() {
            [nameof(User.ObjectId)] = user.ObjectId,
            [nameof(User.Email)] = user.Email,
        });

        if (!await _dbContext.Users.AnyAsync(u => u.ObjectId == user.ObjectId))
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            
            using var userEntityAfterCreateScope = _logger.BeginScope(
                new Dictionary<string, object?>() { [nameof(User.UserId)] = user.UserId });

            _logger.LogInformation("User created.");
        }
    }

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value 
        ?? throw new InvalidOperationException($"Missing required claim '{claimType}'");
}
