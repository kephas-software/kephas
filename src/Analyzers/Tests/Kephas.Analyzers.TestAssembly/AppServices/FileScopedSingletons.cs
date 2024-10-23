using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[SingletonService]
public interface IFileScopedNsSingletonServiceContract
{
}

public class FileScopedNsSingletonService : IFileScopedNsSingletonServiceContract
{
}