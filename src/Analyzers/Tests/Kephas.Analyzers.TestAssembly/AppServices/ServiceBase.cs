using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

[TransientService]
public abstract class ServiceBase
{
}

public class DerivedService : ServiceBase
{
}