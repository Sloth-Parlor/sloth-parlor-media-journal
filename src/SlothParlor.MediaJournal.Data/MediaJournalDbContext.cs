using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Data;

public class MediaJournalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<WatchGroup> WatchGroups { get; set; }
    public DbSet<WatchGroupParticipant> WatchGroupParticipants { get; set; }
    public DbSet<MediaLog> MediaLogs { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<EntryAttendee> EntryAttendees { get; set; }

    public MediaJournalDbContext(DbContextOptions<MediaJournalDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.Schema.Default);

        modelBuilder.Entity<WatchGroup>()
            .HasMany(wg => wg.MediaLogs)
            .WithOne(ml => ml.WatchGroup);

        modelBuilder.Entity<WatchGroup>()
            .HasMany(wg => wg.Participants)
            .WithOne(p => p.WatchGroup);

        modelBuilder.Entity<WatchGroupParticipant>()
            .HasOne(p => p.User)
            .WithMany();

        modelBuilder.Entity<MediaLog>()
            .HasMany(ml => ml.LogEntries)
            .WithOne(e => e.MediaLog);

        modelBuilder.Entity<Entry>()
            .HasMany(e => e.Attendees)
            .WithOne(ea => ea.Entry);
    }
}
