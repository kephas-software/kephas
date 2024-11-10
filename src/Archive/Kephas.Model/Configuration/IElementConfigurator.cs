// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for model element configurators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Configuration
{
    using Kephas.Model.Construction;

    /// <summary>
    /// Contract for model element configurators.
    /// </summary>
    public interface IElementConfigurator
    {
        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        void Configure(IModelConstructionContext constructionContext, INamedElement element);
    }
}