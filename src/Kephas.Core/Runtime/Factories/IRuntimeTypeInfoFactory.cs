// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeTypeInfoFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.Factories
{
    using System;

    /// <summary>
    /// Contract for factories creating <see cref="IRuntimeTypeInfo"/> instances.
    /// </summary>
    public interface IRuntimeTypeInfoFactory : IRuntimeElementInfoFactory<IRuntimeTypeInfo, Type>
    {
    }

    /// <summary>
    /// Base class for <see cref="IRuntimeTypeInfoFactory"/> implementations.
    /// </summary>
    public abstract class RuntimeTypeInfoFactoryBase : RuntimeElementInfoFactoryBase<IRuntimeTypeInfo, Type>, IRuntimeTypeInfoFactory
    {
    }
}