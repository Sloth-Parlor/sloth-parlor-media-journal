using System.Text.Json;
using AutoMapper;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using SlothParlor.MediaJournal.Core.Journal;
using SlothParlor.MediaJournal.Data.Models;
using Xunit.Abstractions;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.RepositoryTests;

[Collection(nameof(CommonTestResourcesCollection))]
public class MediaJournalRepositoryTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly IMapper _mapper;
    private DbContainerDescriptor _db = null!;
    private IMediaLogRepositoryFactory _repositoryFactory = null!;

    public MediaJournalRepositoryTests(ITestOutputHelper output, ApplicationMapperFixture mapperFixture)
    {
        _output = output;
        _mapper = mapperFixture.Mapper;
    }

    public async Task InitializeAsync()
    {
        _db = await DbContainerFactory.Default.CreateAsync();
        _output.WriteLine("DB container stats:\n{0}",
            JsonSerializer.Serialize(
                _db.Stats,
                options: new() { WriteIndented = true }));

        _repositoryFactory = new MediaLogRepositoryFactory(_db.AppDbContext, _mapper);

        await CreateStandardTestData();
    }


    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public async Task WorkbenchTest()
    {
    }

    private async Task CreateStandardTestData()
    {
        var user = new User()
        {
            UserId = Guid.NewGuid().ToString(),
            Email = "testuser1@dev.slothparlor.com",
        };

        await _db.AppDbContext.AddAsync(user);

        await _db.AppDbContext.SaveChangesAsync();
    }
}
