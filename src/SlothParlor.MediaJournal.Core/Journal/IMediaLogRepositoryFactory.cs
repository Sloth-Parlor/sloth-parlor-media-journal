namespace SlothParlor.MediaJournal.Core.Journal;

public interface IMediaLogRepositoryFactory
{
    IMediaLogRepository Create(int watchGroupId);
}
