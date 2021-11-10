﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataTypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Helper extension methods for metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Helper extension methods for metadata.
    /// </summary>
    public static class MetadataTypeExtensions
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
        public static TValue? ExtractMetadataValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueExtractor, TValue? defaultValue = default)
          where TAttribute : Attribute
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            valueExtractor = valueExtractor ?? throw new ArgumentNullException(nameof(valueExtractor));

            var attr = type.GetCustomAttribute<TAttribute>();
            return attr == null ? defaultValue : valueExtractor(attr);
        }
    }
}