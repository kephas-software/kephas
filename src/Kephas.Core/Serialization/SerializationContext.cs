// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A serialization context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// A serialization context.
    /// </summary>
    public class SerializationContext : ContextBase, ISerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext"/> class.
        /// </summary>
        /// <param name="formatType">Type of the format.</param>
        public SerializationContext(Type formatType)
        {
            Contract.Requires(formatType != null);

            this.FormatType = formatType;
        }

        /// <summary>
        /// Gets the type of the format.
        /// </summary>
        /// <value>
        /// The type of the format.
        /// </value>
        public Type FormatType { get; }

        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        public Type RootObjectType { get; set; }
    }

    /// <summary>
    /// A serialization context for the specified format type.
    /// </summary>
    /// <typeparam name="TFormat">The format type.</typeparam>
    public class SerializationContext<TFormat> : SerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext{TFormat}"/> class.
        /// </summary>
        public SerializationContext()
            : base(typeof(TFormat))
        {
        }
    }

    /// <summary>
    /// A serialization context for the specified format type.
    /// </summary>
    /// <typeparam name="TFormat">The format type.</typeparam>
    /// <typeparam name="TRootObject">Type of the root object.</typeparam>
    public class SerializationContext<TFormat, TRootObject> : SerializationContext<TFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext{TFormat, TRootObject}"/> class.
        /// </summary>
        public SerializationContext()
        {
            this.RootObjectType = typeof(TRootObject);
        }
    }
}