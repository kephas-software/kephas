// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEntityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime entity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Runtime
{
    using System;

    using Kephas.Data.Reflection;
    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the runtime entity.
    /// </summary>
    public class RuntimeEntityInfo : RuntimeTypeInfo, IEntityInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEntityInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimeEntityInfo(IRuntimeTypeRegistry typeRegistry, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
        }
    }
}