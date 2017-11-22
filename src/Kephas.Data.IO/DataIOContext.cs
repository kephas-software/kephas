// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data i/o context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// A data I/O context.
    /// </summary>
    public class DataIOContext : Context, IDataIOContext
    {
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