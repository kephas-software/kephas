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

    public class GenericService<TName, TValue> : IGenericContractDeclaration<TName, TValue>
    {
    }

    public class NonServiceButNonGenericContract : INonGenericContract
    {
    }
}