// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using global::ServiceStack.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// A JSON serializer based on the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class JsonSerializer : ISerializer<JsonMediaType>, ISyncSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerConfigurator">The JSON serializer configurator.</param>
        public JsonSerializer(IJsonSerializerConfigurator jsonSerializerConfigurator)
        {
            Requires.NotNull(jsonSerializerConfigurator, nameof(jsonSerializerConfigurator));

            jsonSerializerConfigurator.ConfigureJsonSerialization();
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<JsonSerializer> Logger { get; set; }

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
        public async Task SerializeAsync(object obj, TextWriter textWriter, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            var indent = context?.Indent ?? false;
            if (!indent)
            {
                global::ServiceStack.Text.JsonSerializer.SerializeToWriter(obj, textWriter);
            }
            else
            {
                var serializedObject = global::ServiceStack.Text.JsonSerializer.SerializeToString(obj);
                await textWriter.WriteAsync(serializedObject.IndentJson());
            }
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
        public Task<object> DeserializeAsync(TextReader textReader, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textReader, nameof(textReader));

            // do not call in a new Thread as it may lead to unexpected results. ServiceStack is not thread safe.
            var obj = this.Deserialize(textReader, context);
            return Task.FromResult(obj);
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        public void Serialize(object obj, TextWriter textWriter, ISerializationContext context = null)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            var indent = context?.Indent ?? false;
            if (!indent)
            {
                global::ServiceStack.Text.JsonSerializer.SerializeToWriter(obj, textWriter);
            }
            else
            {
                var serializedObject = global::ServiceStack.Text.JsonSerializer.SerializeToString(obj);
                textWriter.Write(serializedObject.IndentJson());
            }
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object Deserialize(TextReader textReader, ISerializationContext context = null)
        {
            Requires.NotNull(textReader, nameof(textReader));

            var rootObjectType = context?.RootObjectType ?? typeof(object);

            var obj = global::ServiceStack.Text.JsonSerializer.DeserializeFromReader(textReader, rootObjectType);
            if (rootObjectType == typeof(object) && obj != null)
            {
                obj = JsonExpando.TryConvertToJsonExpando(obj);
            }

            return obj;
        }
    }
}