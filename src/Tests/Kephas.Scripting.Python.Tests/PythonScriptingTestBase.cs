namespace Kephas.Scripting.Python.Tests;

using System.Reflection;
using Kephas.Configuration;
using Kephas.Testing;
using Kephas.Testing.Services;

public abstract class PythonScriptingTestBase  : TestBase
{
    protected override IEnumerable<Assembly> GetAssemblies()
    {
        return new List<Assembly>(base.GetAssemblies())
        {
            typeof(IConfiguration<>).Assembly,      // Kephas.Configuration
            typeof(IScriptProcessor).Assembly,      // Kephas.Scripting
            typeof(PythonLanguageService).Assembly, // Kephas.Scripting.Python
        };
    }
}