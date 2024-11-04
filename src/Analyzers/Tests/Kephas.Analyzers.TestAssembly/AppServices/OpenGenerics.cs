using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[ScopedService(RegistrationKind.AsOpenGeneric)]
public interface IOpenGenericContract<TValue>
{
}

public class OpenGenericService<TValue> : IOpenGenericContract<TValue>
{
}