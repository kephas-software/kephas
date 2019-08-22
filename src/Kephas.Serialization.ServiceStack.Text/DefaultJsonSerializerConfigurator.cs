// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultJsonSerializerConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default JSON serializer configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using global::ServiceStack.Text;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Serialization.ServiceStack.Text.Composition;
    using Kephas.Serialization.ServiceStack.Text.Resources;
    using Kephas.Serialization.ServiceStack.Text.TypeSerializers;
    using Kephas.Services;

    /// <summary>
    /// A default JSON serializer configurator.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultJsonSerializerConfigurator : Loggable, IJsonSerializerConfigurator
    {
        /// <summary>
        /// The configure type serialization method.
        /// </summary>
        private static readonly MethodInfo ConfigureTypeSerializationMethod = ReflectionHelper.GetGenericMethodOf(
            _ => ((DefaultJsonSerializerConfigurator)null).ConfigureTypeSerialization<int>(null));

        /// <summary>
        /// The type serializer factories.
        /// </summary>
        private readonly ICollection<IExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>> typeSerializerFactories;

        /// <summary>
        /// True if this object is configured.
        /// </summary>
        private bool isConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializerConfigurator"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="typeSerializerFactories">Optional. The type serializer factories.</param>
        public DefaultJsonSerializerConfigurator(ITypeResolver typeResolver, ICollection<IExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>> typeSerializerFactories = null)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));

            this.typeSerializerFactories = typeSerializerFactories ?? this.GetDefaultTypeSerializers();
            this.TypeResolver = typeResolver;
            this.TypeAttr = "$type";
        }

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        /// <value>
        /// The type resolver.
        /// </value>
        public ITypeResolver TypeResolver { get; }

        /// <summary>
        /// Gets or sets the type attribute.
        /// </summary>
        /// <value>
        /// The type attribute.
        /// </value>
        public string TypeAttr { get; protected set; }

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
                    this.Logger.Warn(Strings.DefaultJsonSerializerConfigurator_ConfigureJsonSerialization_OverwriteSkipped_Warning);
                    return false;
                }

                this.Logger.Debug(Strings.DefaultJsonSerializerConfigurator_ConfigureJsonSerialization_Overwrite_Message);
            }

            // https://groups.google.com/forum/#!topic/servicestack/Ymoug9a0MA8
            // The ServiceStack de/serialization is not safe in async scenarios
            // because it is thread bound.
            // For example, the reason for IncludeTypeInfo = true is to be able to properly deserialize on the client site
            // without knowing the type of the deserialized object.
            JsConfig.TypeAttr = this.TypeAttr;
            JsConfig.IncludeTypeInfo = true;
            JsConfig.TypeWriter = t => t.FullName;
            JsConfig.AllowRuntimeType = t => true;

            // try to avoid StackOverflow exceptions even if this is not what one
            // would really need, better a normal exception
            // https://forums.servicestack.net/t/circular-references-in-jsonserializer-and-stackoverflow-exceptions/5725/18
            JsConfig.MaxDepth = 100;

            JsConfig.EmitCamelCaseNames = true;
            JsConfig.PropertyConvention = PropertyConvention.Lenient;
            JsConfig.ExcludeDefaultValues = false;
            JsConfig.ConvertObjectTypesIntoStringDictionary = true;
            JsConfig.TryToParsePrimitiveTypeValues = false; // otherwise, for untyped jsons, any string value which can't be converted to primitive types will return null.
            JsConfig.DateHandler = DateHandler.ISO8601;

            JsConfig.ThrowOnDeserializationError = false;
            JsConfig.OnDeserializationError = (instance, type, name, str, exception) =>
            {
                this.Logger.Error(exception, Strings.DefaultJsonSerializerConfigurator_OnDeserialization_Error, instance, type, name, str);
                throw exception;
            };

            this.ConfigureTypeFinder();

            foreach (var typeSerializerFactory in this.typeSerializerFactories)
            {
                var typeSerializer = typeSerializerFactory.CreateExportedValue();
                var configureTypeSerialization = ConfigureTypeSerializationMethod.MakeGenericMethod(typeSerializerFactory.Metadata.ValueType);
                configureTypeSerialization.Call(this, typeSerializer);
            }

            this.isConfigured = true;

            return true;
        }

        /// <summary>
        /// Configures the type finder.
        /// </summary>
        private void ConfigureTypeFinder()
        {
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
                                this.Logger.Warn(Strings.DefaultJsonSerializerConfigurator_TypeFinder_TypeNotResolved, typeName);
                            }
                        }

                        return type;
                    }
                    catch (Exception exception)
                    {
                        this.Logger.Error(exception, Strings.DefaultJsonSerializerConfigurator_TypeFinder_Exception, typeName);
                        throw;
                    }
                };
        }

        /// <summary>
        /// Gets the default type serializers.
        /// </summary>
        /// <returns>
        /// The default type serializers.
        /// </returns>
        private ICollection<IExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>> GetDefaultTypeSerializers()
        {
            return new List<IExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>>()
                       {
                           new ExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>(
                               () => new ExpandoSerializer(),
                               new TypeJsonSerializerMetadata(typeof(Expando))),
                           new ExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>(
                               () => new ExpandoInterfaceSerializer(),
                               new TypeJsonSerializerMetadata(typeof(IExpando))),
                           new ExportFactory<ITypeJsonSerializer, TypeJsonSerializerMetadata>(
                               () => new JsonExpandoSerializer(),
                               new TypeJsonSerializerMetadata(typeof(JsonExpando))),
                       };
        }

        /// <summary>
        /// Configure custom type serialization.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        private DefaultJsonSerializerConfigurator ConfigureTypeSerialization<TValue>(ITypeJsonSerializer<TValue> serializer)
        {
            JsConfig<TValue>.RawDeserializeFn = serializer.RawDeserialize;
            JsConfig<TValue>.RawSerializeFn = serializer.RawSerialize;

            return this;
        }
    }
}