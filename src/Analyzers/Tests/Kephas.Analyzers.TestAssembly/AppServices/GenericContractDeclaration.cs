using Kephas.DependencyInjection;

namespace Kephas.Analyzers.TestAssembly;

public interface INonGenericContract
{
} 
    
[TransientService(ServiceType = typeof(INonGenericContract))]
public interface IGenericContractDeclaration<TName, TValue>
{
}

public class NonGenericService : IGenericContractDeclaration<string, int>
{
}

public class NonServiceButNonGenericContract : INonGenericContract
{
}