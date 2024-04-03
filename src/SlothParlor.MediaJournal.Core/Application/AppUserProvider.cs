using Microsoft.AspNetCore.Http;
using SlothParlor.MediaJournal.Core.Extensions;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Application;

public class AppUserProvider : IAppUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MediaJournalDbContext _dbContext;

    private string? _currentUserId;
    private User? _currentUser;

    public AppUserProvider(IHttpContextAccessor httpContextAccessor, MediaJournalDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public string CurrentUserId => _currentUserId 
        ??= _httpContextAccessor.HttpContext.User.GetAppUserId();

    public User CurrentUser => _currentUser
        ??= _dbContext.Users.Find(CurrentUserId) 
        ?? throw new InvalidOperationException("The current user was not set and could not be found.");

    public void SetCurrentUser(User user)
    {
        if (_currentUser is not null)
        {
            if (user.UserId != _currentUser.UserId)
            {
                throw new InvalidOperationException("Current user cannot be reassigned.");
            }

            return;
        }

        if (_currentUserId is not null && user.UserId != _currentUserId)
        {
            throw new InvalidOperationException("Current user does not match claims principal.");
        }

        _currentUser = user;
    }
}