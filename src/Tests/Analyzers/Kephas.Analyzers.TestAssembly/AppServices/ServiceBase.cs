using Kephas.Services;

namespace Kephas.Analyzers.TestAssembly
{
    [AppServiceContract]
    public abstract class ServiceBase
    {
    }

    public class DerivedService : ServiceBase
    {
    }
}
