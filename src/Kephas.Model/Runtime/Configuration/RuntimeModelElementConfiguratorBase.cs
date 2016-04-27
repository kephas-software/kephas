// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConfiguratorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base configurator for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Model.Configuration;

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
        /// Gets the element to be configured.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public TElement Element { get; private set; }

        /// <summary>
        /// Provides the model element to be configured.
        /// </summary>
        /// <param name="element">The model element.</param>
        /// <returns>A configurator for the provided model element.</returns>
        public TConfigurator With(INamedElement element)
        {
            this.Element = (TElement)element;

            return (TConfigurator)(object)this;
        }

        /// <summary>
        /// Provides the model element to be configured.
        /// </summary>
        /// <param name="element">The model element.</param>
        /// <returns>A configurator for the provided model element.</returns>
        IElementConfigurator IElementConfigurator.With(INamedElement element)
        {
            return this.With(element);
        }

        /// <summary>
        /// Configures the model element provided with the.
        /// </summary>
        public abstract void Configure();
    }
}