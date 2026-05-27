using System.Reflection;
using TestExamIS.Tests.Utils;

namespace TestExamIS.Tests;

public class GlobalTestFixture : IAsyncLifetime
{
    private readonly ExamLogger _logger = new("PRACTICE_EXAM", "YOUR_INDEX_HERE",
        logFilePath: Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName ?? "",
            "TestOutput", "test_results.json"));
    public ExamLogger Logger => _logger;

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync()
    {
        await _logger.SaveToFileAsync();
        _logger.PrintSummary();
        _logger.FlushOutput();
    }

    public void BeginTest(string testName) { }
    public void EndTest(string testName, string category, int points, bool passed, string? errorMessage = null)
        => _logger.LogTestResult(testName, category, passed, errorMessage, points);

    public (string category, int points) GetTestMetadata(string testName, object testClassInstance)
    {
        var method = testClassInstance.GetType().GetMethod(testName);
        var attr = method?.GetCustomAttribute<LoggedFactAttribute>();
        return attr != null ? (attr.Category, attr.Points) : ("Default", 1);
    }
}

[CollectionDefinition("Test Suite", DisableParallelization = true)]
public class TestSuiteCollection : ICollectionFixture<GlobalTestFixture> { }
