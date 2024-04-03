using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Application;

public interface IAppUserProvider
{
    string CurrentUserId { get; }

    User CurrentUser { get; }
    
    void SetCurrentUser(User user);
}
