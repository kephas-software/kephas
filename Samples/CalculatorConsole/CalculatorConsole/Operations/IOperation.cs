namespace CalculatorConsole.Operations
{
    using Kephas.Services;

    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(OperationAttribute) })]
    public interface IOperation
    {
        int Compute(int op1, int op2);
    }
}