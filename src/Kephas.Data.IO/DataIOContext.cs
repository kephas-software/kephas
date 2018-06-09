﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Composition;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// A data I/O context.
    /// </summary>
    public class DataIOContext : Context, IDataIOContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition (optional).</param>
        public DataIOContext(ICompositionContext compositionContext = null)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        public Type RootObjectType { get; set; }

        /// <summary>
        /// Gets or sets the serialization context configuration.
        /// </summary>
        /// <value>
        /// The serialization context configuration.
        /// </value>
        public Action<ISerializationContext> SerializationContextConfig { get; set; }
    }
}