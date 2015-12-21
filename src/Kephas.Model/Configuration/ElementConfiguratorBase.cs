// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementConfiguratorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base configurator for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Configuration
{
    /// <summary>
    /// Base configurator for model elements.
    /// </summary>
    /// <typeparam name="TNativeElement">The type of the native element.</typeparam>
    /// <typeparam name="TConfigurator">The type of the configurator.</typeparam>
    public abstract class ElementConfiguratorBase<TNativeElement, TConfigurator> : IElementConfigurator<TNativeElement>
    {
        /// <summary>
        /// Provides the model element to be configured.
        /// </summary>
        /// <param name="element">The model element.</param>
        /// <returns>A configurator for the provided model element.</returns>
        public IElementConfigurator With(INamedElement element)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Configures the model element provided with the.
        /// </summary>
        public void Configure()
        {
            throw new System.NotImplementedException();
        }
    }
}