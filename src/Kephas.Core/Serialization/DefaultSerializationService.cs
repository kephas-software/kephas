// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A default serialization service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Resources;
    using Kephas.Serialization.Composition;
    using Kephas.Serialization.Json;

    /// <summary>
    /// A default serialization service.
    /// </summary>
    public class DefaultSerializationService : ISerializationService
    {
        /// <summary>
        /// The serializer factories.
        /// </summary>
        private readonly IDictionary<Type, IExportFactory<ISerializer, SerializerMetadata>> serializerFactories = new Dictionary<Type, IExportFactory<ISerializer, SerializerMetadata>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializationService"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serializerFactories">The serializer factories.</param>
        public DefaultSerializationService(
            IAmbientServices ambientServices,
            ICollection<IExportFactory<ISerializer, SerializerMetadata>> serializerFactories)
        {
            Contract.Requires(ambientServices != null);
            Contract.Requires(serializerFactories != null);

            this.AmbientServices = ambientServices;
            foreach (var factory in serializerFactories.OrderBy(f => f.Metadata.OverridePriority))
            {
                if (!this.serializerFactories.ContainsKey(factory.Metadata.FormatType))
                {
                    this.serializerFactories.Add(factory.Metadata.FormatType, factory);
                }
            }
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets a serializer for the provided context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serializer.
        /// </returns>
        public ISerializer GetSerializer(ISerializationContext context = null)
        {
            context = context ?? new SerializationContext(this, typeof(JsonFormat));
            var formatType = context.FormatType ?? typeof(JsonFormat);

            var serializer = this.serializerFactories.TryGetValue(formatType);
            if (serializer == null)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultSerializationService_SerializerNotFound_Exception, formatType));
            }

            return serializer.CreateExport().Value;
        }
    }
}