using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[TransientService]
public interface IGenericContract<TValue>
{
}

public class StringService : IGenericContract<string>
{
}

public class IntService : IGenericContract<int>
{
}