using System.Reflection;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class StaticResourcesFixture
{
    private string _basePath;

    public StaticResourcesFixture()
    {
        _basePath = GetBasePath();
        BasePath = new DirectoryInfo(_basePath);
        PostgresInfrastructureDirectory =
            new DirectoryInfo(Path.Combine(_basePath, Constants.PostgresInfrastructureDirectory));
    }

    public DirectoryInfo BasePath { get; init; }

    public DirectoryInfo PostgresInfrastructureDirectory { get; init; }

    public static string GetBasePath() => Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location ?? Environment.CurrentDirectory)
        ?? throw new InvalidOperationException("Could not establish a base path.");
}