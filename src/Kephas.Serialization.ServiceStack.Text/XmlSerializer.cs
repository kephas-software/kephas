// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the XML serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using global::ServiceStack.Text;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An XML serializer based on the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class XmlSerializer : ISerializer<XmlMediaType>, ISyncSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerConfigurator">The JSON serializer configurator.</param>
        public XmlSerializer(IJsonSerializerConfigurator jsonSerializerConfigurator)
        {
            Requires.NotNull(jsonSerializerConfigurator, nameof(jsonSerializerConfigurator));

            jsonSerializerConfigurator.ConfigureJsonSerialization();
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
        public async Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            // TODO no indentation support
            await ((Action)(() =>
                 {
                     using (this.GetJsConfigScope(context))
                     {
                         global::ServiceStack.Text.XmlSerializer.SerializeToWriter(obj, textWriter);
                     }
                 }))
                .AsAsync(cancellationToken)
                .PreserveThreadContext();
        }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        public Task<string> SerializeAsync(
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                return Task.FromResult<string>(null);
            }

            // TODO no indentation support
            return ((Func<string>)(() =>
                {
                    using (this.GetJsConfigScope(context))
                    {
                        var serializedObject = global::ServiceStack.Text.XmlSerializer.SerializeToString(obj);
                        return serializedObject;
                    }
                }))
                .AsAsync(cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        public void Serialize(object obj, TextWriter textWriter, ISerializationContext context = null)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            // TODO no indentation support
            using (this.GetJsConfigScope(context))
            {
                global::ServiceStack.Text.XmlSerializer.SerializeToWriter(obj, textWriter);
            }
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public string Serialize(object obj, ISerializationContext context = null)
        {
            if (obj == null)
            {
                return null;
            }

            // TODO no indentation support
            using (this.GetJsConfigScope(context))
            {
                var serializedObject = global::ServiceStack.Text.XmlSerializer.SerializeToString(obj);
                return serializedObject;
            }
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(TextReader textReader, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textReader, nameof(textReader));

            return ((Func<object>)(() => this.Deserialize(textReader, context))).AsAsync(cancellationToken);
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(string serializedObj, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            if (serializedObj == null)
            {
                return Task.FromResult<object>(null);
            }

            return ((Func<object>)(() => this.Deserialize(serializedObj, context))).AsAsync(cancellationToken);
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object Deserialize(TextReader textReader, ISerializationContext context = null)
        {
            Requires.NotNull(textReader, nameof(textReader));

            var rootObjectType = context?.RootObjectType ?? typeof(object);

            using (this.GetJsConfigScope(context))
            {
                // TODO better implement
                var obj = global::ServiceStack.Text.XmlSerializer.DeserializeFromString(textReader.ReadToEnd(), rootObjectType);
                return this.PostDeserialize(obj, rootObjectType);
            }
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object Deserialize(string serializedObj, ISerializationContext context = null)
        {
            if (serializedObj == null)
            {
                return null;
            }

            var rootObjectType = context?.RootObjectType ?? typeof(object);

            using (this.GetJsConfigScope(context))
            {
                var obj = global::ServiceStack.Text.XmlSerializer.DeserializeFromString(serializedObj, rootObjectType);
                return this.PostDeserialize(obj, rootObjectType);
            }
        }

        private JsConfigScope GetJsConfigScope(ISerializationContext context)
        {
            return JsConfig.With(
                includeTypeInfo: context?.IncludeTypeInfo);
        }

        private object PostDeserialize(object obj, Type rootObjectType)
        {
            if (rootObjectType == typeof(object) && obj != null)
            {
                obj = JsonExpando.TryConvertToJsonExpando(obj);
            }

            return obj;
        }
    }
}