// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultJsonSerializerConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default JSON serializer configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    using global::ServiceStack.Text;

    using Kephas.Dynamic;

    /// <summary>
    /// A default JSON serializer configurator.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultJsonSerializerConfigurator : IJsonSerializerConfigurator
    {
        /// <summary>
        /// True if this object is configured.
        /// </summary>
        private bool isConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerConfigurator"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        public DefaultJsonSerializerConfigurator(ITypeResolver typeResolver)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));

            this.TypeResolver = typeResolver;
        }

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
        public ILogger<IJsonSerializerConfigurator> Logger { get; set; }

        /// <summary>
        /// Configures the JSON serialization.
        /// </summary>
        /// <param name="overwrite">True to overwrite the configuration, false to preserve it (optional).</param>
        /// <returns>
        /// True if the configuration was changed, false otherwise.
        /// </returns>
        public virtual bool ConfigureJsonSerialization(bool overwrite = false)
        {
            if (this.isConfigured)
            {
                if (!overwrite)
                {
                    // TODO localization
                    this.Logger.Debug("JSON serialization already configured, the configuration will not be overwritten.");
                    return false;
                }

                // TODO localization
                this.Logger.Debug("JSON serialization already configured, will be overwritten.");
            }

            // https://groups.google.com/forum/#!topic/servicestack/Ymoug9a0MA8
            // The ServiceStack de/serialization is not safe in async scenarios
            // because it is thread bound.
            // For example, the reason for IncludeTypeInfo = true is to be able to properly deserialize on the client site
            // without knowing the type of the deserialized object.
            JsConfig.TypeAttr = "$type";
            JsConfig.IncludeTypeInfo = true;
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.PropertyConvention = PropertyConvention.Lenient;
            JsConfig.ExcludeDefaultValues = false;
            JsConfig.ThrowOnDeserializationError = false;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;
            JsConfig.TryToParsePrimitiveTypeValues = false; // otherwise, for untyped jsons, any string value which can't be converted to primitive types will return null.
            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.OnDeserializationError = (instance, type, name, str, exception) =>
            {
                this.Logger.Error(exception, $"Error on deserializing {instance}, type: {type}, name: {name}, str: {str}.");
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
                        type = this.TypeResolver.ResolveType(typeName, false);
                        if (type == null)
                        {
                            this.Logger.Warn($"Could not resolve type {typeName}.");
                        }
                    }

                    return type;
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception, $"Errors occurred when trying to resolve type {typeName}.");
                    throw;
                }
            };
            JsConfig<IExpando>.RawDeserializeFn = json => new JsonExpando(json);
            JsConfig<IExpando>.SerializeFn = expando => global::ServiceStack.Text.JsonSerializer.SerializeToString(expando.ToDictionary());
            JsConfig<Expando>.RawDeserializeFn = json => new JsonExpando(json);
            JsConfig<Expando>.SerializeFn = expando => global::ServiceStack.Text.JsonSerializer.SerializeToString(expando.ToDictionary());

            this.isConfigured = true;

            return true;
        }
    }
}