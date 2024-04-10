namespace SlothParlor.MediaJournal.Core.Journal;

internal interface IMediaLogRepositoryFactory
{
    IMediaLogRepository Create(int watchGroupId);
}
