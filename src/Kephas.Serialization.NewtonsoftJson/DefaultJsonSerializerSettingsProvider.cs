// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultJsonSerializerSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A default JSON serializer settings provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Serialization.Json.Converters;
    using Kephas.Serialization.Json.Logging;
    using Kephas.Serialization.Json.Resources;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A default JSON serializer settings provider.
    /// </summary>
    public class DefaultJsonSerializerSettingsProvider : Loggable, IJsonSerializerSettingsProvider
    {
        private static DefaultJsonSerializerSettingsProvider? instance;
        private readonly ICollection<JsonConverter> jsonConverters;
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="jsonConverters">Optional. The JSON converters.</param>
        public DefaultJsonSerializerSettingsProvider(
            ITypeResolver typeResolver,
            ILogManager logManager,
            ICollection<IJsonConverter>? jsonConverters = null)
            : base(logManager)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));

            this.jsonConverters = jsonConverters?.OfType<JsonConverter>().ToList()
                                    ?? new List<JsonConverter>
                                                 {
                                                     new DateTimeJsonConverter(),
                                                     new TimeSpanJsonConverter(),
                                                     new StringEnumJsonConverter(),
                                                     new TypeJsonConverter(typeResolver),
                                                 };
            this.TypeResolver = typeResolver;
            this.logManager = logManager;
        }

        /// <summary>
        /// Gets the default instance of this settings provider.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static IJsonSerializerSettingsProvider Instance => instance ??= CreateDefaultInstance();

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        /// <value>
        /// The type resolver.
        /// </value>
        public ITypeResolver TypeResolver { get; }

        /// <summary>
        /// Configures the provided json serializer settings.
        /// </summary>
        /// <param name="settings">The serializer settings to configure.</param>
        public void ConfigureJsonSerializerSettings(JsonSerializerSettings settings) =>
            this.ConfigureJsonSerializerSettings(
                settings,
                camelCase: true,
                throwOnMissingMembers: true,
                converters: this.jsonConverters);

        /// <summary>
        /// Configures the json serializer settings.
        /// </summary>
        /// <param name="serializerSettings">The serializer settings to configure.</param>
        /// <param name="camelCase">If set to <c>true</c> the camel case will be used for properties.</param>
        /// <param name="throwOnMissingMembers">If set to <c>true</c> the serializer will throw on missing members.</param>
        /// <param name="converters">The json converters.</param>
        /// <returns>
        /// The provided json serializer settings.
        /// </returns>
        protected virtual JsonSerializerSettings ConfigureJsonSerializerSettings(
            JsonSerializerSettings serializerSettings,
            bool camelCase,
            bool throwOnMissingMembers = true,
            IEnumerable<JsonConverter>? converters = null)
        {
            serializerSettings.NullValueHandling = NullValueHandling.Include;
            ////serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            ////serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            serializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            serializerSettings.MissingMemberHandling = throwOnMissingMembers
                ? MissingMemberHandling.Error
                : MissingMemberHandling.Ignore;
            serializerSettings.Error = this.HandleJsonSerializationError;
            serializerSettings.ContractResolver = this.GetContractResolver(camelCase);
            serializerSettings.SerializationBinder = this.GetSerializationBinder();
            serializerSettings.TraceWriter = new JsonTraceWriter(this.logManager);

            serializerSettings.Converters.AddRange(converters ?? this.jsonConverters);

            return serializerSettings;
        }

        /// <summary>
        /// Gets the serialization binder.
        /// </summary>
        /// <returns>
        /// The serialization binder.
        /// </returns>
        protected virtual ISerializationBinder GetSerializationBinder()
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
        /// Creates a default instance.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        private static DefaultJsonSerializerSettingsProvider CreateDefaultInstance()
        {
            var defaultInstance =
                new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()), LoggingHelper.DefaultLogManager);

            return defaultInstance;
        }
    }
}