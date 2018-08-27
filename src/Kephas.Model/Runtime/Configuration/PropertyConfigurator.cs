// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the property configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using Kephas.Composition.AttributedModel;
    using Kephas.Reflection;

    /// <summary>
    /// A property configurator.
    /// </summary>
    [ExcludeFromComposition]
    public class PropertyConfigurator : RuntimeModelElementConfiguratorBase<IProperty, IPropertyInfo, PropertyConfigurator>
    {
    }

    /// <summary>
    /// A property configurator.
    /// </summary>
    /// <typeparam name="TPropertyType">The property type.</typeparam>
    [ExcludeFromComposition]
    public class PropertyConfigurator<TPropertyType> : PropertyConfigurator
    {
    }
}