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
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization.Json.ContractResolvers;
    using Kephas.Serialization.Json.Converters;
    using Kephas.Serialization.Json.Logging;
    using Kephas.Serialization.Json.Resources;
    using Kephas.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A default JSON serializer settings provider.
    /// </summary>
    public class DefaultJsonSerializerSettingsProvider : Loggable, IJsonSerializerSettingsProvider
    {
        private static DefaultJsonSerializerSettingsProvider? instance;
        private readonly Lazy<ICollection<JsonConverter>> lazyJsonConverters;
        private readonly ILogManager? logManager;
        private readonly Lazy<IContractResolver> lazyCamelCaseResolver;
        private readonly Lazy<IContractResolver> lazyInvariantCaseResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerSettingsProvider"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="jsonConverters">Optional. The JSON converters.</param>
        public DefaultJsonSerializerSettingsProvider(
            ITypeResolver typeResolver,
            IRuntimeTypeRegistry typeRegistry,
            ILogManager? logManager,
            ICollection<IExportFactory<IJsonConverter, AppServiceMetadata>>? jsonConverters = null)
            : base(logManager)
        {
            typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));

            this.TypeResolver = typeResolver;
            this.TypeRegistry = typeRegistry;
            this.logManager = logManager;
            this.lazyJsonConverters = new Lazy<ICollection<JsonConverter>>(() => this.ComputeJsonConverters(jsonConverters));
            this.lazyCamelCaseResolver = new Lazy<IContractResolver>(() => new CamelCaseContractResolver(this.lazyJsonConverters.Value));
            this.lazyInvariantCaseResolver = new Lazy<IContractResolver>(() => new InvariantCaseContractResolver(this.lazyJsonConverters.Value));
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
        protected ITypeResolver TypeResolver { get; }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Configures the provided json serializer settings.
        /// </summary>
        /// <remarks>
        /// By default, camel casing is used along with collected json converters.
        /// Additionally missing members generate exceptions.
        /// </remarks>
        /// <param name="settings">The serializer settings to configure.</param>
        public virtual void ConfigureJsonSerializerSettings(JsonSerializerSettings settings) =>
            this.ConfigureJsonSerializerSettings(
                settings,
                camelCase: true,
                throwOnMissingMembers: true,
                converters: this.lazyJsonConverters.Value);

        /// <summary>
        /// Gets the default JSON converters.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <returns>The default JSON converters.</returns>
        protected virtual IEnumerable<JsonConverter> GetDefaultJsonConverters(ITypeResolver typeResolver, IRuntimeTypeRegistry typeRegistry)
        {
            // The order in this list is important, as generic collections should be processed last.
            return new List<JsonConverter>
            {
                new DateTimeJsonConverter(),
                new TimeSpanJsonConverter(),
                new StringEnumJsonConverter(),
                new TypeJsonConverter(typeResolver),
                new DynamicTypeRegistryJsonConverter(typeRegistry, typeResolver),
                new AppServiceMetadataJsonConverter(typeRegistry, typeResolver),
                new AppServiceInfoJsonConverter(typeRegistry, typeResolver),
                new ExpandoJsonConverter(typeRegistry, typeResolver),
                new DictionaryJsonConverter(typeRegistry, typeResolver),
                new AnonymousClassJsonConverter(typeRegistry),
                new ArrayJsonConverter(),
                new CollectionJsonConverter(typeRegistry),
                new ObjectJsonConverter(),
                new DelegateJsonConverter(),
            };
        }

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
            bool throwOnMissingMembers,
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

            serializerSettings.Converters.AddRange(converters ?? this.lazyJsonConverters.Value.Where(c => !c.CanRead || !c.CanWrite));

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
            return camelCase ? this.lazyCamelCaseResolver.Value : this.lazyInvariantCaseResolver.Value;
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
        /// Computes the JSON converters based on the provided export factories.
        /// </summary>
        /// <param name="jsonConverters">The JSON converter export factories.</param>
        /// <returns>A collection of JSON converters.</returns>
        protected virtual ICollection<JsonConverter> ComputeJsonConverters(ICollection<IExportFactory<IJsonConverter, AppServiceMetadata>>? jsonConverters)
        {
            var converters = jsonConverters?
                                 .Order()
                                 .Select(f => f.CreateExportedValue())
                                 .OfType<JsonConverter>()
                                 .ToList()
                             ?? new List<JsonConverter>();
            if (converters.Count == 0)
            {
                converters.AddRange(this.GetDefaultJsonConverters(this.TypeResolver, this.TypeRegistry));
            }

            return converters;
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
                new DefaultJsonSerializerSettingsProvider(new DefaultTypeResolver(() => AppDomain.CurrentDomain.GetAssemblies()), RuntimeTypeRegistry.Instance, LoggingHelper.DefaultLogManager);

            return defaultInstance;
        }
    }
}