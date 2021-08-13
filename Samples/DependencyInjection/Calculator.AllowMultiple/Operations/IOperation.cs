namespace CalculatorConsole.Operations
{
    using Kephas.Services;

    [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(OperationAttribute) })]
    public interface IOperation
    {
        double Compute(double op1, double op2);
    }
}