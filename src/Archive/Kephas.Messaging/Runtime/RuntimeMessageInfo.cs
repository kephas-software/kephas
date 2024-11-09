// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMessageInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime message information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Logging;
    using Kephas.Messaging.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the runtime message.
    /// </summary>
    public class RuntimeMessageInfo : RuntimeTypeInfo, IMessageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeMessageInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimeMessageInfo(IRuntimeTypeRegistry typeRegistry, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
        }
    }
}