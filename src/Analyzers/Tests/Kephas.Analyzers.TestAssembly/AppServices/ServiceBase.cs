using Kephas.DependencyInjection;
using Kephas.Services;

namespace Kephas.Analyzers.TestAssembly;

[TransientService]
public abstract class ServiceBase
{
}

public class DerivedService : ServiceBase
{
}