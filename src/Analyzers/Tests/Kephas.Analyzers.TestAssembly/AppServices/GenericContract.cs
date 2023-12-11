namespace Kephas.Analyzers.TestAssembly
{
    using Kephas.Services;

    [AppServiceContract]
    public interface IGenericContract<TValue>
    {
    }

    public class StringService : IGenericContract<string>
    {
    }

    public class IntService : IGenericContract<int>
    {
    }
}