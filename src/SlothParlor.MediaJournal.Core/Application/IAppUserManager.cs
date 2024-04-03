using System.Security.Claims;

namespace SlothParlor.MediaJournal.Core.Application;

public interface IAppUserManager
{
    Task<Data.Models.User> GetOrCreateUserAsync(string objectId, ClaimsPrincipal claimsPrincipal);
}
