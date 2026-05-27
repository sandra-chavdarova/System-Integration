using System.Text;
using System.Text.Json;

namespace TestExamIS.Tests.Utils;

public class ExamLogger
{
    private readonly List<TestResult> _testResults = new();
    private readonly StringBuilder _output = new();
    private readonly string _logFilePath;
    private readonly string _studentId;

    public ExamLogger(string examId, string studentId, string logFilePath)
    {
        _studentId = studentId;
        _logFilePath = logFilePath;
    }

    public void LogTestResult(string testName, string category, bool isPassed, string? errorMessage = null, int pointsWorth = 1)
    {
        _testResults.Add(new TestResult
        {
            TestName = testName, StudentId = _studentId, TestCategory = category,
            IsPassed = isPassed, ExecutedAt = DateTime.Now, ErrorMessage = errorMessage, Points = pointsWorth,
        });
        _output.Append($"[{DateTime.Now:HH:mm:ss}] [{category}] {testName} ({pointsWorth} pts): ");
        _output.AppendLine(isPassed ? "PASSED" : $"FAILED\n - {errorMessage}");
    }

    public void PrintSummary()
    {
        var passed = _testResults.Count(r => r.IsPassed);
        var earned = _testResults.Where(r => r.IsPassed).Sum(r => r.Points);
        var total = _testResults.Sum(r => r.Points);
        _output.AppendLine($"\n========== TEST SUMMARY ==========");
        _output.AppendLine($"Passed: {passed}/{_testResults.Count} | Points: {earned}/{total}");
        foreach (var cat in _testResults.Select(r => r.TestCategory).Distinct())
        {
            var ct = _testResults.Where(r => r.TestCategory == cat).ToList();
            var cp = ct.Where(r => r.IsPassed).Sum(r => r.Points);
            var ctp = ct.Sum(r => r.Points);
            _output.AppendLine($"  {cat}: {cp}/{ctp} pts");
        }
        _output.AppendLine("==================================");
    }

    public void FlushOutput() { Console.WriteLine(_output.ToString()); _output.Clear(); }

    public async Task SaveToFileAsync()
    {
        var json = JsonSerializer.Serialize(_testResults, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
        await File.WriteAllTextAsync(_logFilePath, json);
    }
}
