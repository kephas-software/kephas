// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   JSON serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Serialization.Formats;

    using Newtonsoft.Json;

    /// <summary>
    /// JSON serializer.
    /// </summary>
    public class JsonSerializer : ISerializer<JsonFormat>
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
        /// Serializes the provided object asynchronously as JSON.
        /// </summary>
        /// <param name="obj">              The object.</param>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public Task<string> SerializeAsync(
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var settings = this.settingsProvider.GetJsonSerializerSettings();
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

            return Task.Factory.StartNew(
                () =>
                    {
                        var sb = new StringBuilder();
                        using (var stringStream = new StringWriter(sb))
                        {
                            serializer.Serialize(stringStream, obj);
                        }

                        return sb.ToString();
                    },
                cancellationToken);
        }

        /// <summary>
        /// Deserialize an object asynchronously from the provided JSON.
        /// </summary>
        /// <param name="serializedObj">    The serialized object.</param>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var settings = this.settingsProvider.GetJsonSerializerSettings();
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

            return Task.Factory.StartNew(
                () =>
                    {
                        object result;
                        using (var streamReader = new StringReader(serializedObj))
                        using (var jsonReader = new JsonTextReader(streamReader))
                        {
                            result = serializer.Deserialize(jsonReader);
                        }

                        return result;
                    }, 
                cancellationToken);
        }
    }
}