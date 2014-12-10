using System.Reflection;
using Kephas.Services;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Contract for element factories.
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// Tries to create an element based on the native element.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        /// <returns>A new element based on the provided native element, or <c>null</c> if the native element is not supported.</returns>
        INamedElement TryCreateElement(MemberInfo nativeElement);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TElement">The type of the element being created.</typeparam>
    [SharedAppServiceContract(AllowMultiple = true, ContractType = typeof(IElementFactory),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IElementFactory<TElement> : IElementFactory
    {
    }
}