using System.Reflection;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Base implementation of an element factory.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TElementConstructorInfo">The type of the element constructor information.</typeparam>
    public abstract class ElementFactoryBase<TElement, TElementConstructorInfo> : IElementFactory
        where TElement : INamedElement
        where TElementConstructorInfo : ElementConstructorInfo
    {
        /// <summary>
        /// Tries to create an element based on the native element.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        /// <returns>A new element based on the provided native element, or <c>null</c> if the native element is not supported.</returns>
        public INamedElement TryCreateElement(MemberInfo nativeElement)
        {
            var elementConstructorInfo = this.TryGetElementConstructorInfo(nativeElement);

            if (elementConstructorInfo == null)
            {
                return null;
            }

            var element = this.CreateElement(elementConstructorInfo);
            return element;
        }

        /// <summary>
        /// Tries to get the element constructor information.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        /// <returns>The element constructor information, if available, otherwise <c>null</c>.</returns>
        protected abstract TElementConstructorInfo TryGetElementConstructorInfo(MemberInfo nativeElement);

        /// <summary>
        /// Creates the element based on the provided constructor information.
        /// </summary>
        /// <param name="elementConstructorInfo">The element constructor information.</param>
        /// <returns>The newly created element.</returns>
        protected abstract TElement CreateElement(TElementConstructorInfo elementConstructorInfo);
    }
}