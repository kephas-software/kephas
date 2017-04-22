// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    [ContractClass(typeof(RuntimeModelElementConstructorContractClass))]
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
        INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement);

        /// <summary>
        /// Tries to compute the name for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A string containing the name, or <c>null</c> if the name could not be computed.
        /// </returns>
        string TryComputeName(IModelConstructionContext constructionContext, object runtimeElement);
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TModelContract">The model contract type.</typeparam>
    /// <typeparam name="TRuntime">The runtime type.</typeparam>
    [SharedAppServiceContract(
        AllowMultiple = true, 
        ContractType = typeof(IRuntimeModelElementConstructor))]
    public interface IRuntimeModelElementConstructor<TModel, TModelContract, TRuntime> : IRuntimeModelElementConstructor
        where TModel : class, INamedElement
        where TModelContract : class, INamedElement
        where TRuntime : class
    {
    }

    /// <summary>
    /// Contract class for <see cref="IRuntimeModelElementConstructor"/>.
    /// </summary>
    [ContractClassFor(typeof(IRuntimeModelElementConstructor))]
    internal abstract class RuntimeModelElementConstructorContractClass : IRuntimeModelElementConstructor
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided runtime element
        /// information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>if
        /// the runtime element information is not supported.
        /// </returns>
        public INamedElement TryCreateModelElement(IModelConstructionContext constructionContext, object runtimeElement)
        {
            Requires.NotNull(constructionContext, nameof(constructionContext));
            Requires.NotNull(runtimeElement, nameof(runtimeElement));

            return Contract.Result<INamedElement>();
        }

        /// <summary>
        /// Tries to compute the name for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A string containing the name, or <c>null</c> if the name could not be computed.
        /// </returns>
        public string TryComputeName(IModelConstructionContext constructionContext, object runtimeElement)
        {
            Requires.NotNull(constructionContext, nameof(constructionContext));
            Requires.NotNull(runtimeElement, nameof(runtimeElement));

            return Contract.Result<string>();
        }
    }
}