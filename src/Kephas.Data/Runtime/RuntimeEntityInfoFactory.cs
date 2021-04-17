// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEntityInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data type information factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Runtime
{
    using System;

    using Kephas.Runtime;
    using Kephas.Runtime.Factories;

    /// <summary>
    /// A data type information factory.
    /// </summary>
    public class RuntimeEntityInfoFactory : RuntimeTypeInfoFactoryBase
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="args">Additional arguments.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, params object[] args)
        {
            if (typeof(IEntity).IsAssignableFrom(reflectInfo))
            {
                return new RuntimeEntityInfo(registry, reflectInfo);
            }

            return null;
        }
    }
}