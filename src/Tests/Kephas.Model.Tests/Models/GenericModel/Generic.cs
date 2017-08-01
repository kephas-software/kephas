namespace Kephas.Model.Tests.Models.GenericModel
{
    interface IComplex<T1, T2>
    {
        T1 Value1 { get; set; }
        T2 Value2 { get; set; }
    }

    interface IIntComplex : IComplex<int, int>
    {
    }

    interface IFloatComplex : IComplex<float, float>
    {
    }
}