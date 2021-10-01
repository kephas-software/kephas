// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Base class for runtime element infos.
    /// </summary>
    public abstract class RuntimeElementInfoBase : Expando
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected readonly ILogger? Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeElementInfoBase"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry containing this element.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected RuntimeElementInfoBase(
            IRuntimeTypeRegistry typeRegistry,
            ILogger? logger = null)
            : base(isThreadSafe: true)
        {
            typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));

            this.TypeRegistry = typeRegistry;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the container type registry.
        /// </summary>
        public IRuntimeTypeRegistry TypeRegistry { get; }
    }
}