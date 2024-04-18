using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlothParlor.MediaJournal.Data;

namespace SlothParlor.MediaJournal.Core.Journal;

public class MediaLogRepository : IMediaLogRepository
{
    private readonly MediaJournalDbContext _dbContext;
    private readonly IMapper _mapper;

    public MediaLogRepository(MediaJournalDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;

        _mapper = mapper;
    }

    public required int WatchGroupId { get; init; }

    public async Task<Contracts.MediaLog.MediaLogResult> CreateEmptyAsync(Contracts.MediaLog.MediaLogInput properties)
    {
        var data = _mapper.Map<Data.Models.MediaLog>(properties);

        data.WatchGroupId = WatchGroupId;

        var result = await _dbContext.AddAsync(data);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.MediaLog.MediaLogResult>(result.Entity);
    }

    public async Task<Contracts.MediaLog.MediaLogResult> DeleteAsync(int mediaLogId)
    {
        var result = _dbContext.Remove(await FindAsync(mediaLogId));

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.MediaLog.MediaLogResult>(result.Entity);
    }

    public async Task<IReadOnlyCollection<Contracts.MediaLog.MediaLogResult>> GetAsync() =>
        await _dbContext.MediaLogs
            .Where(ml => ml.WatchGroupId == WatchGroupId)
            .ProjectTo<Contracts.MediaLog.MediaLogResult>(_mapper.ConfigurationProvider)
            .ToArrayAsync();

    public async Task<Contracts.MediaLog.MediaLogResult> GetAsync(int mediaLogId) =>
        _mapper.Map<Contracts.MediaLog.MediaLogResult>(await FindAsync(mediaLogId));

    public async Task<Contracts.MediaLog.MediaLogResult> UpdateAsync(
        int mediaLogId,
        Contracts.MediaLog.MediaLogInput properties,
        Func<EntityEntry<Data.Models.MediaLog>, Task>? changeTracking = null)
    {
        var data = await FindAsync(mediaLogId);

        data.DisplayName = properties.DisplayName;

        var result = _dbContext.Update(data);

        await changeTracking?.Invoke(result);
        
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.MediaLog.MediaLogResult>(result.Entity);
    }

    private async Task<Data.Models.MediaLog> FindAsync(int mediaLogId)
    {
        var result = await _dbContext.MediaLogs.FindAsync(mediaLogId);

        if (result is null || result.WatchGroupId != WatchGroupId)
        {
            throw new InvalidOperationException(
                $"Media log with Id `{mediaLogId}` does not exist or is not associated with watch group `{WatchGroupId}`.");
        }

        return result;
    }
}