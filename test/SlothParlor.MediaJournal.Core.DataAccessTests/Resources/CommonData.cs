using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public record CommonData(
    WatchGroup DefaultWatchGroup, 
    MediaLog DefaultMediaLog)
{ 
    public static async Task<CommonData> Create(MediaJournalDbContext dbContext)
    {
        User testuser1 = new()
        {
            UserId = Guid.NewGuid().ToString(),
            Email = "testuser1@dev.slothparlor.com",
        };

        await dbContext.AddAsync(testuser1);

        WatchGroup testUser1DefaultWatchGroup = new()
        {
            DisplayName = "Default",
            Owners = [testuser1],
        };

        var watchGroupEntityResult = await dbContext.AddAsync(testUser1DefaultWatchGroup);

        WatchGroupParticipant[] Participants = [
            new()
            {
                UserId = testuser1.UserId,
                WatchGroup = testUser1DefaultWatchGroup,
                DisplayName = "Participant 1",
            },
        ];

        await dbContext.AddRangeAsync(Participants);

        MediaLog defaultWatchGroupDefaultMediaLog = new()
        {
            DisplayName = "Default",
            WatchGroup = testUser1DefaultWatchGroup,
            LogEntries = [
                new()
                {
                    CandidateName = "Knives Out",
                    Attendees = [
                        new EntryAttendee()
                        {
                            Participant = Participants[0],
                        }
                    ]
                },
                new()
                {
                    CandidateName = "Jurrasic Park",
                    Attendees = [
                        new EntryAttendee()
                        {
                            Participant = Participants[0],
                        }
                    ]
                }
            ]
        };

        var mediaLogEntityResult = await dbContext.AddAsync(defaultWatchGroupDefaultMediaLog);

        await dbContext.SaveChangesAsync();

        watchGroupEntityResult.State = EntityState.Detached;
        mediaLogEntityResult.State = EntityState.Detached;

        return new(
            watchGroupEntityResult.Entity,
            mediaLogEntityResult.Entity);
    }
}
