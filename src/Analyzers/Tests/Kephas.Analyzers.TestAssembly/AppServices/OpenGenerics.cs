using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[ScopedService(GenericRegistration.AsOpen)]
public interface IOpenGenericContract<TValue>
{
}

public class OpenGenericService<TValue> : IOpenGenericContract<TValue>
{
}