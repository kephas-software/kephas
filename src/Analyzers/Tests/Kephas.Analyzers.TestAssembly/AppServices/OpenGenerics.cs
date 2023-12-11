namespace Kephas.Analyzers.TestAssembly
{
    using Kephas.Services;

    [ScopedAppServiceContract(AsOpenGeneric = true)]
    public interface IOpenGenericContract<TValue>
    {
    }

    public class OpenGenericService<TValue> : IOpenGenericContract<TValue>
    {
    }
}