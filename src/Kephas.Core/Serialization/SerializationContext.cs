﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A serialization context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// A serialization context.
    /// </summary>
    public class SerializationContext : Context, ISerializationContext
    {
        /// <summary>
        /// The serialization service.
        /// </summary>
        private ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext"/> class.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="mediaType">The media type (type implementing <see cref="IMediaType"/>).</param>
        public SerializationContext(ISerializationService serializationService = null, Type mediaType = null)
            : base(compositionContext: serializationService?.CompositionContext)
        {
            this.SerializationService = serializationService;
            this.MediaType = mediaType;
        }

        /// <summary>
        /// Gets or sets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        public ISerializationService SerializationService
        {
            get => this.serializationService;
            set
            {
                Requires.NotNull(value, nameof(value));

                this.serializationService = value;
                this.SetCompositionContext(value.CompositionContext);
            }
        }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        public Type MediaType { get; set; }

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
        /// Gets or sets a value indicating whether the serialized output should be indented.
        /// </summary>
        /// <value>
        /// True if the output should be indented, false if not.
        /// </value>
        public bool Indent { get; set; }

        /// <summary>
        /// Creates a new configured <see cref="SerializationContext"/>.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="rootObjectFactory">The root object factory (optional).</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TMediaType>(ISerializationService serializationService, Func<object> rootObjectFactory = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return new SerializationContext(serializationService, typeof(TMediaType)) { RootObjectFactory = rootObjectFactory };
        }

        /// <summary>
        /// Creates a new configured <see cref="SerializationContext"/>.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="rootObjectFactory">The root object factory (optional).</param>
        /// <returns>
        /// A configured <see cref="SerializationContext"/>.
        /// </returns>
        public static SerializationContext Create<TMediaType, TRootObject>(ISerializationService serializationService, Func<object> rootObjectFactory = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return new SerializationContext(serializationService, typeof(TMediaType)) { RootObjectType = typeof(TRootObject), RootObjectFactory = rootObjectFactory };
        }
    }
}