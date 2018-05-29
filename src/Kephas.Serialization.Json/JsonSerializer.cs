// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   JSON serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Services;

    using Newtonsoft.Json;

    /// <summary>
    /// JSON serializer.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class JsonSerializer : ISerializer<JsonMediaType>
    {
        /// <summary>
        /// The settings provider.
        /// </summary>
        private readonly IJsonSerializerSettingsProvider settingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/>
        /// class.
        /// </summary>
        /// <param name="settingsProvider">The settings provider.</param>
        public JsonSerializer(IJsonSerializerSettingsProvider settingsProvider = null)
        {
            this.settingsProvider = settingsProvider ?? DefaultJsonSerializerSettingsProvider.Instance;
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
        public Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            var settings = this.settingsProvider.GetJsonSerializerSettings();
            if (context?.Indent ?? false)
            {
                settings.Formatting = Formatting.Indented;
            }

            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

            return Task.Factory.StartNew(
                () =>
                {
                    serializer.Serialize(textWriter, obj);
                },
                cancellationToken);
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
        public Task<object> DeserializeAsync(
            TextReader textReader,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textReader, nameof(textReader));

            var settings = this.settingsProvider.GetJsonSerializerSettings();
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

            return Task.Factory.StartNew(
                () =>
                {
                    object result;
                    using (var jsonReader = new JsonTextReader(textReader))
                    {
                        result = context?.RootObjectFactory?.Invoke();
                        if (result != null)
                        {
                            serializer.Populate(jsonReader, result);
                        }
                        else
                        {
                            result = context?.RootObjectType != null
                                         ? serializer.Deserialize(jsonReader, context.RootObjectType)
                                         : serializer.Deserialize(jsonReader);
                        }
                    }

                    return result;
                },
                cancellationToken);
        }
    }
}