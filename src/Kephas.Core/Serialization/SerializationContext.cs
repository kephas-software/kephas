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
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="formatType">Type of the format.</param>
        public SerializationContext(ISerializationService serializationService, Type formatType)
            : base(serializationService.AmbientServices)
        {
            Contract.Requires(serializationService != null);
            Contract.Requires(formatType != null);

            this.SerializationService = serializationService;
            this.FormatType = formatType;
        }

        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        public ISerializationService SerializationService { get; }

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
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="rootObjectFactory">The root object factory (optional).</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TFormatType>(ISerializationService serializationService, Func<object> rootObjectFactory = null)
            where TFormatType : IFormat
        {
            Contract.Requires(serializationService != null);

            return new SerializationContext(serializationService, typeof(TFormatType)) { RootObjectFactory = rootObjectFactory };
        }

        /// <summary>
        /// Creates a new configured <see cref="SerializationContext"/>.
        /// </summary>
        /// <typeparam name="TFormatType">The format type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="rootObjectFactory">The root object factory (optional).</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TFormatType, TRootObject>(ISerializationService serializationService, Func<object> rootObjectFactory = null)
            where TFormatType : IFormat
        {
            Contract.Requires(serializationService != null);

            return new SerializationContext(serializationService, typeof(TFormatType)) { RootObjectType = typeof(TRootObject), RootObjectFactory = rootObjectFactory };
        }
    }
}