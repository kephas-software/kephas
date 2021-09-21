namespace Kephas.Analyzers.TestAssembly
{
    using Kephas.Services;

    public interface INonGenericContract
    {
    } 
    
    [AppServiceContract(ContractType = typeof(INonGenericContract))]
    public interface IGenericContractDeclaration<TValue>
    {
    }

    public class GenericService<TValue> : IGenericContractDeclaration<TValue>
    {
    }

    public class NonServiceButNonGenericContract : INonGenericContract
    {
    }
}