using System.Reflection;
using Kephas.Services;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Contract for element factories.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, 
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IElementFactory
    {
        /// <summary>
        /// Tries to create an element based on the native element.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        /// <returns>A new element based on the provided native element, or <c>null</c> if the native element is not supported.</returns>
        INamedElement TryCreateElement(MemberInfo nativeElement);
    }
}