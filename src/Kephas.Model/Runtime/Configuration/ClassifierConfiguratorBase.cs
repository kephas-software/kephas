// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConfiguratorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using System;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Resources;

    /// <summary>
    /// A classifier configurator.
    /// </summary>
    /// <typeparam name="TElement">Type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    /// <typeparam name="TConfigurator">Type of the configurator.</typeparam>
    public abstract class ClassifierConfiguratorBase<TElement, TRuntimeElement, TConfigurator> : RuntimeModelElementConfiguratorBase<TElement, TRuntimeElement, TConfigurator>
        where TConfigurator : IRuntimeModelElementConfigurator<TElement, TRuntimeElement>
        where TElement : IClassifier
    {
        /// <summary>
        /// Configures the provided property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyConfig">The property configuration.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        public TConfigurator WithProperty(string propertyName, Action<PropertyConfigurator> propertyConfig)
        {
            Requires.NotNull(propertyName, nameof(propertyName));
            Requires.NotNull(propertyConfig, nameof(propertyConfig));

            this.AddConfiguration(
                (context, element) =>
                    {
                        var property = ((IModelElement)element).GetDeclaredMembers().OfType<IProperty>().FirstOrDefault(p => p.Name == propertyName);
                        if (property == null)
                        {
                            throw new ModelConfigurationException(propertyName, element.Name, Strings.ClassifierConfiguratorBase_WithProperty_ForeignProperty_Exception);
                        }

                        var propertyConfigurator = new PropertyConfigurator();
                        propertyConfig(propertyConfigurator);

                        propertyConfigurator.Configure(context, property);
                    });

            return (TConfigurator)(object)this;
        }
    }
}