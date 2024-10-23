using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[ScopedService(AsOpenGeneric = true)]
public interface IOpenGenericContract<TValue>
{
}

public class OpenGenericService<TValue> : IOpenGenericContract<TValue>
{
}