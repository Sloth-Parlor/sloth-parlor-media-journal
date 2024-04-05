namespace SlothParlor.MediaJournal.Contracts.WatchGroup
{
    public record WatchGroupParticipantInput
    {
        public string UserId { get; init; }

        public int WatchGroupId { get; init; }

        public string DisplayName { get; init; }
    }
}