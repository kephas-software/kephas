// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConfiguratorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base configurator for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Model.Configuration;
    using Kephas.Model.Construction;

    /// <summary>
    /// Base configurator for model elements.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    /// <typeparam name="TConfigurator">The type of the configurator.</typeparam>
    public abstract class RuntimeModelElementConfiguratorBase<TElement, TRuntimeElement, TConfigurator> : IRuntimeModelElementConfigurator<TElement, TRuntimeElement>
        where TElement : INamedElement
        where TConfigurator : IRuntimeModelElementConfigurator<TElement, TRuntimeElement>
    {
        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        void IElementConfigurator.Configure(IModelConstructionContext constructionContext, INamedElement element)
        {
            this.Configure(constructionContext, (TElement)element);
        }

        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        public abstract void Configure(IModelConstructionContext constructionContext, TElement element);
    }
}