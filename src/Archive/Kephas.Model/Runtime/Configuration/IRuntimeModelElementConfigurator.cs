// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelElementConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        /// <summary>
        /// Adds a member to the configured element.
        /// </summary>
        /// <param name="member">The member to be added.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        IRuntimeModelElementConfigurator AddMember(INamedElement member);

        /// <summary>
        /// Adds a member out of the runtime element to the configured model element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        IRuntimeModelElementConfigurator AddMember(object runtimeElement);
    }

    /// <summary>
    /// Generic contract for model element configurators.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">The type of the runtime element.</typeparam>
    [SingletonAppServiceContract(
        AllowMultiple = true,
        ContractType = typeof(IRuntimeModelElementConfigurator))]
    public interface IRuntimeModelElementConfigurator<TElement, TRuntimeElement> : IRuntimeModelElementConfigurator
    {
    }
}