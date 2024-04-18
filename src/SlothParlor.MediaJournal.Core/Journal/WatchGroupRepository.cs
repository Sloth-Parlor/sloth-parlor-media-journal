using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlothParlor.MediaJournal.Data;

namespace SlothParlor.MediaJournal.Core.Journal;

public class WatchGroupRepository : IWatchGroupRepository
{
    private readonly MediaJournalDbContext _dbContext;
    private readonly IMapper _mapper;

    private Data.Models.User? _user = null;

    public WatchGroupRepository(MediaJournalDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;

        _mapper = mapper;
    }

    public required string UserId { get; init; }

    public async Task<Contracts.WatchGroup.WatchGroupResult> CreateAsync(Contracts.WatchGroup.WatchGroupInput properties)
    {
        var data = _mapper.Map<Data.Models.WatchGroup>(properties);

        // TODO: decide how to handle display names for new watch groups; might make sense in the context of a UI
        var implicitDisplayName = User.Email;

        data.Participants = [
            new()
            {
                UserId = User.UserId,
                DisplayName = implicitDisplayName
            }
        ];

        var result = await _dbContext.AddAsync(data);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.WatchGroup.WatchGroupResult>(result.Entity);
    }

    public async Task<Contracts.WatchGroup.WatchGroupResult> DeleteAsync(int watchGroupId)
    {
        var result = _dbContext.Remove(await FindAsync(watchGroupId));

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.WatchGroup.WatchGroupResult>(result.Entity);
    }

    public async Task<IReadOnlyCollection<Contracts.WatchGroup.WatchGroupResult>> GetAsync()
    {
        return await _dbContext.WatchGroups
            .Include(wg => wg.Participants)
            .Where(wg => wg.Participants!.Any(p => p.UserId == UserId))
            .ProjectTo<Contracts.WatchGroup.WatchGroupResult>(_mapper.ConfigurationProvider)
            .ToArrayAsync();
    }

    public async Task<Contracts.WatchGroup.WatchGroupResult> GetAsync(int watchGroupId) =>
        _mapper.Map<Contracts.WatchGroup.WatchGroupResult>(await FindAsync(watchGroupId));

    public async Task<Contracts.WatchGroup.WatchGroupResult> UpdateAsync(
        int watchGroupId,
        Contracts.WatchGroup.WatchGroupInput properties,
        Func<EntityEntry<Data.Models.WatchGroup>, Task>? on = null)
    {
        var data = await FindAsync(watchGroupId);

        data.DisplayName = properties.DisplayName;

        var result = _dbContext.Update(data);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.WatchGroup.WatchGroupResult>(result.Entity);
    }

    private Data.Models.User User => _user 
        ??= _dbContext.Users.Find(UserId) 
        ?? throw new InvalidOperationException($"User with Id `{UserId}` does not exist.");

    private async Task<Data.Models.WatchGroup> FindAsync(int watchGroupId)
    {
        var result = await _dbContext.WatchGroups.FindAsync(watchGroupId);

        if (result is null || result.Participants?.Select(p => p.UserId).Contains(UserId) is false)
        {
            throw new InvalidOperationException(
                $"Media log with Id `{watchGroupId}` does not exist or is not associated with user `{UserId}`.");
        }

        return result;
    }
}