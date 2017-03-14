// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultJsonSerializerSettingsProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A default JSON serializer settings provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
        /// The formatter assembly style value.
        /// </summary>
        private static readonly object FormatterAssemblyStyleDefaultValue;

        /// <summary>
        /// Information describing the formatter assembly style dynamic property.
        /// </summary>
        private static readonly IRuntimePropertyInfo FormatterAssemblyStyleRuntimePropertyInfo;

        /// <summary>
        /// The JSON converters.
        /// </summary>
        private readonly ICollection<JsonConverter> jsonConverters;

        /// <summary>
        /// Initializes static members of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        static DefaultJsonSerializerSettingsProvider()
        {
            var ambientServices = AmbientServices.Instance;
            Instance = new DefaultJsonSerializerSettingsProvider(ambientServices.GetService<ITypeResolver>())
            {
                Logger = ambientServices.GetLogger<DefaultJsonSerializerSettingsProvider>()
            };

            var dummySettings = new JsonSerializerSettings();
            FormatterAssemblyStyleRuntimePropertyInfo =
                dummySettings.GetRuntimeTypeInfo().Properties["TypeNameAssemblyFormat"];
            FormatterAssemblyStyleDefaultValue = Enum.Parse(FormatterAssemblyStyleRuntimePropertyInfo.PropertyInfo.PropertyType, "Simple");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="jsonConverters">The JSON converters (optional).</param>
        public DefaultJsonSerializerSettingsProvider(ITypeResolver typeResolver, ICollection<IJsonConverter> jsonConverters = null)
        {
            this.jsonConverters = jsonConverters?.OfType<JsonConverter>().ToList()
                                    ?? new List<JsonConverter>
                                                 {
                                                     new DateTimeJsonConverter(),
                                                     new TimeSpanJsonConverter(),
                                                     new StringEnumJsonConverter(),
                                                 };
            this.TypeResolver = typeResolver;
        }

        /// <summary>
        /// Gets the default instance of this settings provider.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DefaultJsonSerializerSettingsProvider Instance { get; }

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        /// <value>
        /// The type resolver.
        /// </value>
        public ITypeResolver TypeResolver { get; }

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
        protected virtual JsonSerializerSettings GetJsonSerializerSettings(
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
                MissingMemberHandling = thrownOnMissingMembers
                                            ? MissingMemberHandling.Error
                                            : MissingMemberHandling.Ignore,
                Error = this.HandleJsonSerializationError,
                ContractResolver = this.GetContractResolver(camelCase),
                Binder = this.GetSerializationBinder(),
            };

            serializerSettings.Converters.AddRange(converters ?? this.jsonConverters);

            this.SetDefaultTypeNameAssemblyFormat(serializerSettings);

            return serializerSettings;
        }

        /// <summary>
        /// Gets the serialization binder.
        /// </summary>
        /// <returns>
        /// The serialization binder.
        /// </returns>
        protected virtual SerializationBinder GetSerializationBinder()
        {
            return new TypeResolverSerializationBinder(this.TypeResolver);
        }

        /// <summary>
        /// Gets the contract resolver.
        /// </summary>
        /// <param name="camelCase">If set to <c>true</c> the camel case will be used for properties.</param>
        /// <returns>
        /// The contract resolver.
        /// </returns>
        protected virtual IContractResolver GetContractResolver(bool camelCase)
        {
            Contract.Ensures(Contract.Result<IContractResolver>() != null);

            return camelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver();
        }

        /// <summary>
        /// Error handler for json deserialization errors.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleJsonSerializationError(object sender, ErrorEventArgs args)
        {
            this.Logger.Error(
                args.ErrorContext.Error,
                Strings.DefaultJsonSerializerSettingsProvider_ErrorOnSerializingObjectMessage,
                args.CurrentObject?.GetType());
        }

        /// <summary>
        /// Sets the default type name assembly format.
        /// </summary>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <returns>
        /// The JsonSerializerSettings.
        /// </returns>
        private JsonSerializerSettings SetDefaultTypeNameAssemblyFormat(JsonSerializerSettings serializerSettings)
        {
            // WORAROUND for a PCL problem in Newtonsoft.Json component
            // See http://stackoverflow.com/questions/27080363/missingmethodexception-with-newtonsoft-json-when-using-typenameassemblyformat-wi
            // for more information.
            FormatterAssemblyStyleRuntimePropertyInfo.SetValue(serializerSettings, FormatterAssemblyStyleDefaultValue);
            return serializerSettings;
        }
    }
}