// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattingHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Helper class for formatting.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Internals
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Helper class for formatting.
    /// </summary>
    internal static class FormattingHelper
    {
        /// <summary>
        /// Formats the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string containing the formatted type.</returns>
        public static string Format(Type type)
        {
            Requires.NotNull(type, nameof(type));

            if (type.IsConstructedGenericType)
            {
                return FormatClosedGeneric(type);
            }

            return type.Name;
        }

        /// <summary>
        /// Formats the closed generic type.
        /// </summary>
        /// <param name="closedGenericType">The constructed generic type.</param>
        /// <returns>A string containing the formatted type.</returns>
        public static string FormatClosedGeneric(Type closedGenericType)
        {
            Requires.NotNull(closedGenericType, nameof(closedGenericType));
            if (!closedGenericType.IsConstructedGenericType)
            {
                // TODO localization
                throw new ArgumentException("Please provide a constructed generic type.", nameof(closedGenericType));
            }

            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            var name = closedGenericType.Name.Substring(0, closedGenericType.Name.IndexOf("`"));
            var args = closedGenericType.GenericTypeArguments.Select(Format);
            return $"{name}<{string.Join(", ", args)}>";
        }
    }
}