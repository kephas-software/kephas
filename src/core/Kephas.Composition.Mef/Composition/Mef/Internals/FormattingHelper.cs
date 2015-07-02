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
            Contract.Requires(type != null);

            if (type.IsConstructedGenericType)
            {
                return FormatClosedGeneric(type);
            }

            return type.Name;
        }

        /// <summary>
        /// Formats the closed generic type.
        /// </summary>
        /// <param name="closedGenericType">Type of the closed generic.</param>
        /// <returns>A string containing the formatted type.</returns>
        public static string FormatClosedGeneric(Type closedGenericType)
        {
            Contract.Requires(closedGenericType != null);
            Contract.Requires(closedGenericType.IsConstructedGenericType);

            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            var name = closedGenericType.Name.Substring(0, closedGenericType.Name.IndexOf("`"));
            var args = closedGenericType.GenericTypeArguments.Select(Format);
            return string.Format("{0}<{1}>", name, string.Join(", ", args));
        }
    }
}