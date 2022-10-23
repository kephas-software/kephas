// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the property configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.AttributedModel;

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Reflection;

    /// <summary>
    /// A property configurator.
    /// </summary>
    [ExcludeFromInjection]
    public class PropertyConfigurator : RuntimeModelElementConfiguratorBase<IProperty, IPropertyInfo, PropertyConfigurator>
    {
    }

    /// <summary>
    /// A property configurator.
    /// </summary>
    /// <typeparam name="TPropertyType">The property type.</typeparam>
    [ExcludeFromInjection]
    public class PropertyConfigurator<TPropertyType> : PropertyConfigurator
    {
    }
}