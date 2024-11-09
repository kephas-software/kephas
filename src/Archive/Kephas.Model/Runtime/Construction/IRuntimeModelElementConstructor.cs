// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    public interface IRuntimeModelElementConstructor
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided runtime element
        /// information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        INamedElement? TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement);

        /// <summary>
        /// Tries to compute the name for the provided runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// A string containing the name, or <c>null</c> if the name could not be computed.
        /// </returns>
        string? TryComputeName(object runtimeElement, IModelConstructionContext constructionContext);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TModelContract">The model contract type.</typeparam>
    /// <typeparam name="TRuntime">The runtime type.</typeparam>
    [SingletonAppServiceContract(
        AllowMultiple = true, 
        ContractType = typeof(IRuntimeModelElementConstructor))]
    public interface IRuntimeModelElementConstructor<TModel, TModelContract, TRuntime> : IRuntimeModelElementConstructor
        where TModel : class, INamedElement
        where TModelContract : class, INamedElement
        where TRuntime : class
    {
    }
}