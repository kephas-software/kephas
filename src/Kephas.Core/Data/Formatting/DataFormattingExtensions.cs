﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataFormattingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Formatting
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Extension methods for data formatting.
    /// </summary>
    public static class DataFormattingExtensions
    {
        /// <summary>
        /// Converts the object to a serialization friendly representation.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        public static object? ToData(this object? obj, IDataFormattingContext? context = null)
        {
            return obj == null
                ? null
                : obj is IDataFormattable formattable
                    ? formattable.ToData(context)
                    : obj is Exception ex
                        ? (object)new ExceptionData(ex)
                        : obj.ToString();
        }
    }
}