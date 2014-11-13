// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper extension methods for metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Metadata
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Helper extension methods for metadata.
    /// </summary>
    public static class MetadataExtensions
    {
        /// <summary>
        /// Extracts the metadata value from the attribute with the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="valueExtractor">The value extractor.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The metadata value.</returns>
        public static TValue ExtractMetadataValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueExtractor, TValue defaultValue = default(TValue))
          where TAttribute : Attribute
        {
            Contract.Requires(type != null);
            Contract.Requires(valueExtractor != null);

            var attr = type.GetTypeInfo().GetCustomAttribute<TAttribute>();
            if (attr == null)
            {
                return defaultValue;
            }

            return valueExtractor(attr);
        }

        /// <summary>
        /// Extracts the metadata value from the attribute with the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The metadata value.</returns>
        public static TValue ExtractMetadataValue<TAttribute, TValue>(this Type type, TValue defaultValue = default(TValue))
          where TAttribute : Attribute, IMetadataValue<TValue>
        {
            Contract.Requires(type != null);

            var attr = type.GetTypeInfo().GetCustomAttribute<TAttribute>();
            if (attr == null)
            {
                return defaultValue;
            }

            return attr.Value;
        }
    }
}