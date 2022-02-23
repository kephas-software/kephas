namespace Kephas.Scripting.Python.Tests;

using System.Reflection;
using Kephas.Configuration;
using Kephas.Testing.Injection;

public abstract class PythonScriptingTestBase : InjectionTestBase
{
    public override IEnumerable<Assembly> GetAssemblies()
    {
        return new List<Assembly>(base.GetAssemblies())
        {
            typeof(IConfiguration<>).Assembly,      // Kephas.Configuration
            typeof(IScriptProcessor).Assembly,      // Kephas.Scripting
            typeof(PythonLanguageService).Assembly  // Kephas.Scripting.Python
        };
    }
}