// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimePropertyInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.Factories
{
    using System.Reflection;

    /// <summary>
    /// Contract for factories creating <see cref="IRuntimePropertyInfo"/> instances.
    /// </summary>
    public interface IRuntimePropertyInfoFactory : IRuntimeElementInfoFactory<IRuntimePropertyInfo, PropertyInfo>
    {
    }

    /// <summary>
    /// Base class for <see cref="IRuntimePropertyInfoFactory"/> implementations.
    /// </summary>
    public abstract class RuntimePropertyInfoFactoryBase : RuntimeElementInfoFactoryBase<IRuntimePropertyInfo, PropertyInfo>, IRuntimePropertyInfoFactory
    {
    }
}