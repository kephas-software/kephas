// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeSettingsInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Runtime
{
    using System;

    using Kephas.Configuration.Reflection;
    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Information about the runtime message.
    /// </summary>
    public class RuntimeSettingsInfo : RuntimeTypeInfo, ISettingsInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeSettingsInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimeSettingsInfo(IRuntimeTypeRegistry typeRegistry, Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
        }
    }
}