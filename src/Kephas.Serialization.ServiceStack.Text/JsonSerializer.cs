// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Services;

    using global::ServiceStack.Text;

    /// <summary>
    /// A JSON serializer based on the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class JsonSerializer : ISerializer<JsonMediaType>
    {
        /// <summary>
        /// True if JSON serialization configured.
        /// </summary>
        private static bool jsonSerializationConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="logger">The logger.</param>
        public JsonSerializer(ITypeResolver typeResolver, ILogger<JsonSerializer> logger)
        {
            this.Logger = logger;

            if (!jsonSerializationConfigured)
            {
                ConfigureJsonSerialization(typeResolver, logger);
                jsonSerializationConfigured = true;
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<JsonSerializer> Logger { get; }

        /// <summary>
        /// Configures the JSON serialization.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="logger">The logger.</param>
        public static void ConfigureJsonSerialization(ITypeResolver typeResolver, ILogger logger)
        {
            // https://groups.google.com/forum/#!topic/servicestack/Ymoug9a0MA8
            // The ServiceStack de/serialization is not safe in async scenarios
            // because it is thread bound.
            // For example, the reason for IncludeTypeInfo = true is to be able to properly deserialize on the client site
            // without knowing the type of the deserialized object.
            JsConfig.IncludeTypeInfo = true;
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.PropertyConvention = PropertyConvention.Lenient;
            JsConfig.ExcludeDefaultValues = false;
            JsConfig.ThrowOnDeserializationError = false;
            JsConfig.OnDeserializationError = (instance, type, name, str, exception) =>
            {
                logger.Error(exception, $"Error on deserializing {instance}, type: {type}, name: {name}, str: {str}.");
                throw exception;
            };
            var originalTypeFinder = JsConfig.TypeFinder;
            JsConfig.TypeFinder = typeName =>
            {
                try
                {
                    var type = originalTypeFinder(typeName);
                    if (type == null)
                    {
                        type = typeResolver.ResolveType(typeName, false);
                        if (type == null)
                        {
                            logger.Warn($"Could not resolve type {typeName}.");
                        }
                    }

                    return type;
                }
                catch (Exception exception)
                {
                    logger.Error(exception, $"Errors occurred when trying to resolve type {typeName}.");
                    throw;
                }
            };
        }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public Task SerializeAsync(object obj, TextWriter textWriter, ISerializationContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            global::ServiceStack.Text.JsonSerializer.SerializeToWriter(obj, textWriter);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(TextReader textReader, ISerializationContext context = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var obj = global::ServiceStack.Text.JsonSerializer.DeserializeFromReader(textReader, context?.RootObjectType ?? typeof(object));
            return Task.FromResult(obj);
        }
    }
}