using System.Text.Json.Serialization;

namespace SlothParlor.MediaJournal.Contracts.WatchGroup;

public record WatchGroupResult
{
    public WatchGroupResult(
        int watchGroupId,
        string displayName)
    {
        WatchGroupId = watchGroupId;
        DisplayName = displayName;
    }

    public WatchGroupResult(
        int watchGroupId,
        string displayName,
        IReadOnlyCollection<WatchGroupParticipantResult>? participants)
    {
        WatchGroupId = watchGroupId;
        DisplayName = displayName;
        Participants = participants;
    }

    public int WatchGroupId { get; }

    public string DisplayName { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<WatchGroupParticipantResult>? Participants { get; } = null;
}