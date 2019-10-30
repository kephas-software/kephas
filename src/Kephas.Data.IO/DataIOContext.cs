// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
        /// <param name="compositionContext">Context for the composition.</param>
        public DataIOContext(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOContext"/> class.
        /// </summary>
        /// <param name="operationContext">Optional. The parent operation context.</param>
        public DataIOContext(IContext operationContext)
            : base(operationContext)
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
        public Action<ISerializationContext> SerializationConfig { get; set; }
    }
}