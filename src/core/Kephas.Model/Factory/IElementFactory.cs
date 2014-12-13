// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Reflection;

    using Kephas.Services;

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

        /// <summary>
        /// Tries to create an element based on the native element.
        /// </summary>
        /// <param name="constructorInfo">The constructor information.</param>
        /// <returns>A new element based on the provided constructor information, or <c>null</c> if the data provided in the constructor information is not supported.</returns>
        INamedElement TryCreateElement(ElementConstructorInfo constructorInfo);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TElement">The type of the element being created.</typeparam>
    /// <typeparam name="TConstructorInfo">The type of the constructor information.</typeparam>
    [SharedAppServiceContract(AllowMultiple = true, ContractType = typeof(IElementFactory),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IElementFactory<TElement, TConstructorInfo> : IElementFactory
        where TConstructorInfo : ElementConstructorInfo
    {
    }
}