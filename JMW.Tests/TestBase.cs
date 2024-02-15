using System.IO;
using System.Reflection;

namespace JMW.Tests;

public class TestBase
{
    public string ExecutionPath;

    public TestBase()
    {
        this.ExecutionPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..");
    }
}
