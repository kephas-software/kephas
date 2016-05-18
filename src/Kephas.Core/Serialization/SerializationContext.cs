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

        /// <summary>
        /// Gets or sets the root object factory.
        /// </summary>
        /// <value>
        /// The root object factory.
        /// </value>
        public Func<object> RootObjectFactory { get; set; }

        /// <summary>
        /// Creates a new configured <see cref="SerializationContext"/>.
        /// </summary>
        /// <typeparam name="TFormatType">Type of the format type.</typeparam>
        /// <param name="rootObjectFactory">The root object factory.</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TFormatType>(Func<object> rootObjectFactory = null)
            where TFormatType : IFormat
        {
            return new SerializationContext(typeof(TFormatType)) { RootObjectFactory = rootObjectFactory };
        }

        /// <summary>
        /// Creates a new configured <see cref="SerializationContext"/>.
        /// </summary>
        /// <typeparam name="TFormatType">The format type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="rootObjectFactory">The root object factory.</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TFormatType, TRootObject>(Func<object> rootObjectFactory = null)
            where TFormatType : IFormat
        {
            return new SerializationContext(typeof(TFormatType)) { RootObjectType = typeof(TRootObject), RootObjectFactory = rootObjectFactory };
        }
    }
}