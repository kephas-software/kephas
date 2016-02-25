// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for element factories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Diagnostics.Contracts;

    using Kephas.Model.Factory;
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
    }

    /// <summary>
    /// Contract for element factories.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TRuntime">The runtime type.</typeparam>
    [SharedAppServiceContract(
        AllowMultiple = true, 
        ContractType = typeof(IRuntimeModelElementConstructor),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IRuntimeModelElementConstructor<TModel, TRuntime> : IRuntimeModelElementConstructor
        where TModel : class, INamedElement
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
            Contract.Requires(constructionContext != null);
            Contract.Requires(runtimeElement != null);

            return Contract.Result<INamedElement>();
        }
    }
}