using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Collections;

[CollectionDefinition(nameof(EmptyPostgresDbCollection))]
public class EmptyPostgresDbCollection
    : ICollectionFixture<EmptyPostgresDbFixture>
{
}
