namespace Kephas.Serialization.Json
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization.Formatters;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Serialization.Json.Converters;
    using Kephas.Serialization.Json.Resources;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A default JSON serializer settings provider.
    /// </summary>
    public class DefaultJsonSerializerSettingsProvider : IJsonSerializerSettingsProvider
    {
        /// <summary>
        /// The JSON converters.
        /// </summary>
        private readonly ICollection<JsonConverter> jsonConverters;

        /// <summary>
        /// Initializes static members of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        static DefaultJsonSerializerSettingsProvider()
        {
            Instance = new DefaultJsonSerializerSettingsProvider
                           {
                               Logger = AmbientServices.Instance.GetLogger<DefaultJsonSerializerSettingsProvider>()
                           };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        /// <param name="jsonConverters">The JSON converters.</param>
        public DefaultJsonSerializerSettingsProvider(ICollection<IJsonConverter> jsonConverters = null)
        {
            this.jsonConverters = jsonConverters?.OfType<JsonConverter>().ToList() 
                                    ?? new List<JsonConverter>
                                                 {
                                                     new DateTimeJsonConverter(),
                                                     new TimeSpanJsonConverter(),
                                                     new StringEnumJsonConverter(),
                                                 };
        }

        /// <summary>
        /// Gets the default instance of this settings provider.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DefaultJsonSerializerSettingsProvider Instance { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultJsonSerializerSettingsProvider> Logger { get; set; }

        /// <summary>
        /// Gets the JSON serializer settings.
        /// </summary>
        /// <returns>
        /// The JSON serializer settings.
        /// </returns>
        public JsonSerializerSettings GetJsonSerializerSettings()
        {
            return this.GetJsonSerializerSettings(camelCase: true, thrownOnMissingMembers: true, converters: this.jsonConverters);
        }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        /// <param name="camelCase">If set to <c>true</c> the ccamel case will be used for properties.</param>
        /// <param name="thrownOnMissingMembers">If set to <c>true</c> [thrown on missing members].</param>
        /// <param name="converters">The json converters.</param>
        /// <returns>
        /// The json serializer settings.
        /// </returns>
        protected JsonSerializerSettings GetJsonSerializerSettings(
            bool camelCase,
            bool thrownOnMissingMembers = true,
            IEnumerable<JsonConverter> converters = null)
        {
            var serializerSettings = new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Include,
                                            ////PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                            ////ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                            TypeNameHandling = TypeNameHandling.Objects,
                                            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                            MissingMemberHandling = thrownOnMissingMembers
                                                                        ? MissingMemberHandling.Error
                                                                        : MissingMemberHandling.Ignore,
                                            Error = this.HandleJsonSerializationError,
                                        };

            converters = converters ?? this.jsonConverters;

            serializerSettings.Converters.AddRange(converters);

            // TODO ContractResolver
            ////serializerSettings.ContractResolver = camelCase
            ////                                          ? DefaultClientModelContractResolver.CamelCaseContractResolver
            ////                                          : DefaultClientModelContractResolver.PascalCaseContractResolver;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            serializerSettings.Error = (obj, args) =>
            {
                this.Logger.Error(
                    string.Format(Strings.DefaultJsonSerializerSettingsProvider_ErrorOnSerializingObjectMessage, args.CurrentObject?.GetType()),
                    args.ErrorContext.Error);
            };

            return serializerSettings;
        }

        /// <summary>
        /// Error handler for json deserialization errors.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void HandleJsonSerializationError(object sender, ErrorEventArgs args)
        {
            this.Logger.Warn(args.ErrorContext.Error, Strings.DefaultJsonSerializerSettingsProvider_ErrorOnSerializingMessage, args.CurrentObject);
        }
    }
}