using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class EntryRepository : IEntryRepository
{
    private readonly MediaJournalDbContext _dbContext;
    private readonly IMapper _mapper;

    public EntryRepository(MediaJournalDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public int MediaLogId { get; init; }

    public async Task<Contracts.MediaLog.EntryResult> CreateAsync(Contracts.MediaLog.EntryInput properties)
    {
        var data = _mapper.Map<Data.Models.Entry>(properties);

        data.MediaLogId = MediaLogId;

        var result = await _dbContext.AddAsync(data);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.MediaLog.EntryResult>(result.Entity);
    }

    public async Task DeleteAsync(int entryId)
    {
        var result = _dbContext.Remove(await FindEntryAsync(entryId));

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Contracts.MediaLog.EntryResult>> GetAsync() =>
        await _dbContext.Entries
            .Where(e => e.MediaLogId == MediaLogId)
            .ProjectTo<Contracts.MediaLog.EntryResult>(_mapper.ConfigurationProvider)
            .ToArrayAsync();

    public async Task<Contracts.MediaLog.EntryResult> GetAsync(int entryId) =>
        _mapper.Map<Contracts.MediaLog.EntryResult>(await FindEntryAsync(entryId));

    public async Task<Contracts.MediaLog.EntryResult> UpdateAsync(int entryId, Contracts.MediaLog.EntryInput properties)
    {
        var entryData = await FindEntryAsync(entryId);

        entryData.CandidateName = properties.CandidateName;

        entryData.Attendees = properties.Attendees?
            .Select(_mapper.Map<EntryAttendee>);

        var result = _dbContext.Update(entryData);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Contracts.MediaLog.EntryResult>(result.Entity);
    }

    private async Task<Data.Models.Entry> FindEntryAsync(int entryId)
    {
        var result = await _dbContext.Entries.FindAsync(entryId);

        if (result is null || result.MediaLogId != MediaLogId)
        {
            throw new InvalidOperationException(
                $"Entry with Id `{entryId}` does not exist or is not associated with media log `{MediaLogId}`.");
        }

        return result;
    }
}
