// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model element configurators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Model.Configuration;
    using Kephas.Services;

    /// <summary>
    /// Contract for model element configurators.
    /// </summary>
    public interface IRuntimeModelElementConfigurator : IElementConfigurator
    {
    }

    /// <summary>
    /// Generic contract for model element configurators.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">The type of the runtime element.</typeparam>
    [SharedAppServiceContract(
        AllowMultiple = true,
        ContractType = typeof(IRuntimeModelElementConfigurator),
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IRuntimeModelElementConfigurator<TElement, TRuntimeElement> : IRuntimeModelElementConfigurator
    {
    }
}