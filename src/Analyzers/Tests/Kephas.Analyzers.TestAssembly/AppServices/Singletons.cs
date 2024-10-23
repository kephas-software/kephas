using Kephas.DependencyInjection;
using Kephas.Services;

namespace Kephas.Analyzers.TestAssembly;

[SingletonService]
public interface ISingletonServiceContract
{
}
    
public class SingletonService : ISingletonServiceContract
{
}