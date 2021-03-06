﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEventInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime event information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Runtime
{
    using System;

    using Kephas.Logging;
    using Kephas.Messaging.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the runtime event.
    /// </summary>
    public class RuntimeEventInfo : RuntimeMessageInfo, IEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimeEventInfo(IRuntimeTypeRegistry typeRegistry, Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
        }
    }
}