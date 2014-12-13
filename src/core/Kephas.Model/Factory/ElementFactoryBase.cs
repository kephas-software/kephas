// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementFactoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of an element factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Base implementation of an element factory.
    /// </summary>
    /// <typeparam name="TElement">The type of the element being created.</typeparam>
    /// <typeparam name="TConstructorInfo">The type of the element constructor information.</typeparam>
    public abstract class ElementFactoryBase<TElement, TConstructorInfo> : IElementFactory<TElement, TConstructorInfo>
        where TElement : INamedElement
        where TConstructorInfo : ElementConstructorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementFactoryBase{TElement, TConstructorInfo}"/> class.
        /// </summary>
        /// <param name="modelSpaceProvider">The model space provider.</param>
        protected ElementFactoryBase(IModelSpaceProvider modelSpaceProvider)
        {
            Contract.Requires(modelSpaceProvider != null);

            this.ModelSpaceProvider = modelSpaceProvider;
        }

        /// <summary>
        /// Gets the model space provider.
        /// </summary>
        /// <value>
        /// The model space provider.
        /// </value>
        public IModelSpaceProvider ModelSpaceProvider { get; private set; }

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
        /// Tries to create an element based on the native element.
        /// </summary>
        /// <param name="constructorInfo">The constructor information.</param>
        /// <returns>A new element based on the provided constructor information, or <c>null</c> if the data provided in the constructor information is not supported.</returns>
        public INamedElement TryCreateElement(ElementConstructorInfo constructorInfo)
        {
            var elementConstructorInfo = constructorInfo as TConstructorInfo;
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
        protected abstract TConstructorInfo TryGetElementConstructorInfo(MemberInfo nativeElement);

        /// <summary>
        /// Creates the element based on the provided constructor information.
        /// </summary>
        /// <param name="elementConstructorInfo">The element constructor information.</param>
        /// <returns>The newly created element.</returns>
        protected abstract TElement CreateElement(TConstructorInfo elementConstructorInfo);
    }
}