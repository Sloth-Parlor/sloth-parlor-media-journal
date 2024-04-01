using System.Security.Claims;

namespace SlothParlor.MediaJournal.Core.JournalUser;

public interface IUserService
{
    Task CreateUserIfNotExistsAsync(ClaimsPrincipal claimsPrincipal);
}
