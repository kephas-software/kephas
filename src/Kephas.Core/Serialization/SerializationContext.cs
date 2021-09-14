// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A serialization context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// A serialization context.
    /// </summary>
    public class SerializationContext : Context, ISerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext"/> class.
        /// </summary>
        /// <param name="injector">Context for the composition.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaType">Optional. The media type (type implementing <see cref="IMediaType"/>).</param>
        public SerializationContext(IInjector injector, ISerializationService serializationService, Type? mediaType = null)
            : base(injector)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            this.SerializationService = serializationService;
            this.MediaType = mediaType;
        }

        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        public ISerializationService SerializationService { get; }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        public Type? MediaType { get; set; }

        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        public Type? RootObjectType { get; set; }

        /// <summary>
        /// Gets or sets the root object factory.
        /// </summary>
        /// <value>
        /// The root object factory.
        /// </value>
        public Func<object>? RootObjectFactory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the serialized output should be indented.
        /// </summary>
        /// <value>
        /// True if the output should be indented, false if not.
        /// If a value is not provided, the default serializer settings are used.
        /// </value>
        public bool? Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type information should be included.
        /// </summary>
        /// <value>
        /// True to include type information, false otherwise.
        /// If a value is not provided, the default serializer settings are used.
        /// </value>
        public bool? IncludeTypeInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether null values should be included.
        /// </summary>
        /// <value>
        /// True to include null values, false otherwise.
        /// If a value is not provided, the default serializer settings are used.
        /// </value>
        public bool? IncludeNullValues { get; set; }
    }
}