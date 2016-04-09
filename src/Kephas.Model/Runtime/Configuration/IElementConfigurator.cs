// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model element configurators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Services;

    /// <summary>
    /// Contract for model element configurators.
    /// </summary>
    public interface IElementConfigurator
    {
        /// <summary>
        /// Provides the model element to be configured.
        /// </summary>
        /// <param name="element">The model element.</param>
        /// <returns>A configurator for the provided model element.</returns>
        IElementConfigurator With(INamedElement element);

        /// <summary>
        /// Configures the model element provided with the.
        /// </summary>
        void Configure();
    }

    /// <summary>
    /// Generic contract for model element configurators.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">The type of the runtime element.</typeparam>
    [SharedAppServiceContract(
        AllowMultiple = true,
        ContractType = typeof(IElementConfigurator),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IElementConfigurator<TElement, TRuntimeElement> : IElementConfigurator
    {
    }
}