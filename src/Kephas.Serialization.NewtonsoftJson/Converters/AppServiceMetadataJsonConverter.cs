// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// JSON converter for <see cref="AppServiceMetadata"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="AppServiceMetadata"/> should be processed before expandos, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class AppServiceMetadataJsonConverter : ExpandoJsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadataJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public AppServiceMetadataJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
            : base(typeRegistry, typeResolver, typeof(AppServiceMetadata), typeof(AppServiceMetadata))
        {
        }

        /// <summary>
        /// Creates the expando value which should collect the JSON values.
        /// </summary>
        /// <param name="expandoTypeInfo">The type information of the target expando value.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>The newly created expando collector.</returns>
        protected override IExpandoBase CreateExpandoCollector(IRuntimeTypeInfo expandoTypeInfo, object? existingValue)
        {
            return existingValue == null ? new Expando() : (IExpandoBase)existingValue;
        }

        /// <summary>
        /// Gets a value indicating whether the property can be written.
        /// </summary>
        /// <remarks>
        /// All properties can be written as they are collected in an untyped expando.
        /// </remarks>
        /// <param name="propInfo">The property information.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>A value indicating whether the property can be written.</returns>
        protected override bool CanWriteProperty(IRuntimePropertyInfo propInfo, object? existingValue)
        {
            return existingValue == null || base.CanWriteProperty(propInfo, existingValue);
        }

        /// <summary>
        /// Gets the return value of the <see cref="ExpandoJsonConverter.ReadJson"/> operation.
        /// </summary>
        /// <param name="expandoTypeInfo">The return value type information.</param>
        /// <param name="expandoCollector">The expando value collecting the properties.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>The read operation's return value.</returns>
        protected override object? GetReadReturnValue(IRuntimeTypeInfo expandoTypeInfo, IExpandoBase expandoCollector, object? existingValue)
        {
            return existingValue != null
                ? base.GetReadReturnValue(expandoTypeInfo, expandoCollector, existingValue)
                : expandoTypeInfo.CreateInstance(expandoCollector.ToDictionary());
        }
    }
}