using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Contracts.WatchGroup;

public record WatchGroupResult(
    int WatchGroupId,
    string DisplayName,
    IEnumerable<WatchGroupParticipantResult> Participants);
