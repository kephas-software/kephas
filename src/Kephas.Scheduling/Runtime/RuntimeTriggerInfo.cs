// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTriggerInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime trigger information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Runtime
{
    using System;

    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Scheduling.Reflection;

    /// <summary>
    /// Information about the runtime trigger.
    /// </summary>
    public class RuntimeTriggerInfo : RuntimeTypeInfo, ITriggerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTriggerInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimeTriggerInfo(IRuntimeTypeRegistry typeRegistry, Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
        }
    }
}
