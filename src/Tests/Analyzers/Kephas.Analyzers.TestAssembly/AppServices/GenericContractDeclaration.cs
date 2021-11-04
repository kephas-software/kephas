namespace Kephas.Analyzers.TestAssembly
{
    using Kephas.Services;

    public interface INonGenericContract
    {
    } 
    
    [AppServiceContract(ContractType = typeof(INonGenericContract))]
    public interface IGenericContractDeclaration<TName, TValue>
    {
    }

    public class NonGenericService : IGenericContractDeclaration<string, int>
    {
    }

    public class NonServiceButNonGenericContract : INonGenericContract
    {
    }
}