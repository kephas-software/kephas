using Kephas.Services;

namespace Kephas.Analyzers.TestAssembly;

[SingletonAppServiceContract]
public interface IFileScopedNsSingletonServiceContract
{
}

public class FileScopedNsSingletonService : IFileScopedNsSingletonServiceContract
{
}