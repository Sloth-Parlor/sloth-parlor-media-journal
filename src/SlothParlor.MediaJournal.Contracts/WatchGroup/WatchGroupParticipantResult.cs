namespace SlothParlor.MediaJournal.Contracts.WatchGroup;

public record WatchGroupParticipantResult(
    int ParticipantId,
    string UserId,
    string DisplayName);