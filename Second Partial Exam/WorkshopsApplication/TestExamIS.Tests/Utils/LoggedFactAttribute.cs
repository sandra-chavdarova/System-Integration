namespace TestExamIS.Tests.Utils;

public class LoggedFactAttribute : FactAttribute
{
    public string Category { get; set; } = "Default";
    public int Points { get; set; } = 1;
}
