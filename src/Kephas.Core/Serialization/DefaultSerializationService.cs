// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A default serialization service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Resources;
    using Kephas.Serialization.Composition;

    /// <summary>
    /// A default serialization service.
    /// </summary>
    public class DefaultSerializationService : ISerializationService, ICompositionContextAware
    {
        private readonly IDictionary<Type, IExportFactory<ISerializer, SerializerMetadata>> serializerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializationService"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="serializerFactories">The serializer factories.</param>
        public DefaultSerializationService(ICompositionContext compositionContext, ICollection<IExportFactory<ISerializer, SerializerMetadata>> serializerFactories)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(serializerFactories, nameof(serializerFactories));

            this.CompositionContext = compositionContext;
            this.serializerFactories = serializerFactories.ToPrioritizedDictionary(f => f.Metadata.MediaType);
        }

        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        public ICompositionContext CompositionContext { get; }

        /// <summary>
        /// Gets a serializer for the provided context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serializer.
        /// </returns>
        public ISerializer GetSerializer(ISerializationContext context = null)
        {
            context = context ?? new SerializationContext(this.CompositionContext, this, typeof(JsonMediaType));
            var mediaType = context.MediaType ?? typeof(JsonMediaType);

            var serializer = this.serializerFactories.TryGetValue(mediaType);
            if (serializer == null)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultSerializationService_SerializerNotFound_Exception, mediaType));
            }

            return serializer.CreateExport().Value;
        }
    }
}