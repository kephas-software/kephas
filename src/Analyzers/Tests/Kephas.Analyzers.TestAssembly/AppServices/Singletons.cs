using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[SingletonService]
public interface ISingletonServiceContract
{
}
    
public class SingletonService : ISingletonServiceContract
{
}