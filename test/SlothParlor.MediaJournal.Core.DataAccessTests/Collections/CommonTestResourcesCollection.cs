using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Collections;

[CollectionDefinition(nameof(CommonTestResourcesCollection))]
public class CommonTestResourcesCollection 
    : ICollectionFixture<StaticResourcesFixture>, ICollectionFixture<ApplicationMapperFixture>
{
}
