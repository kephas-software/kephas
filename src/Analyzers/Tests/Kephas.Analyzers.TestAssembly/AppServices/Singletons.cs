using Kephas.Services;

namespace Kephas.Analyzers.TestAssembly
{
    [SingletonAppServiceContract]
    public interface ISingletonServiceContract
    {
    }
    
    public class SingletonService : ISingletonServiceContract
    {
    }
}
