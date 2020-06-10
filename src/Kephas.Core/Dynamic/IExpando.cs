// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for dynamic objects allowing getting or setting
//   properties by their name through an indexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Contract for dynamic objects allowing getting or setting
    /// properties by their name through an indexer.
    /// </summary>
    public interface IExpando : IDynamicMetaObjectProvider, IIndexable
    {
        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        bool HasDynamicMember(string memberName);

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the
        /// respective properties' values.
        /// </summary>
        /// <param name="keyFunc">The key transformation function (optional).</param>
        /// <param name="valueFunc">The value transformation function (optional).</param>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        IDictionary<string, object?> ToDictionary(
            Func<string, string>? keyFunc = null,
            Func<object?, object?>? valueFunc = null);
    }

    /// <summary>
    /// Extension methods for <see cref="IExpando"/>.
    /// </summary>
    public static class ExpandoExtensions
    {
        private static readonly IDictionary<Type, Func<string, object?>> Parsers = new Dictionary<Type, Func<string, object?>>
        {
            { typeof(bool), v => bool.TryParse(v, out var b) ? (object)b : null },
            { typeof(int), v => int.TryParse(v, out var b) ? (object)b : null },
            { typeof(long), v => long.TryParse(v, out var b) ? (object)b : null },
            { typeof(DateTime), v => DateTime.TryParse(v, out var b) ? (object)b : null },
            { typeof(Guid), v => Guid.TryParse(v, out var b) ? (object)b : null },
        };

        /// <summary>
        /// Merges the indicated options into the context.
        /// </summary>
        /// <typeparam name="T">Type of the context.</typeparam>
        /// <typeparam name="TContract">Type of the expando contract.</typeparam>
        /// <param name="expando">The expando.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// This <paramref name="expando"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Merge<T, TContract>(this T expando, Action<TContract>? optionsConfig)
            where T : class, TContract
            where TContract : IExpando
        {
            Requires.NotNull(expando, nameof(expando));

            optionsConfig?.Invoke(expando);

            return expando;
        }

        /// <summary>
        /// Merges the source object properties into the expando.
        /// </summary>
        /// <remarks>
        /// Collections of key-value pairs (including dictionaries) are merged by their keys, provided the key has the type of string.
        /// </remarks>
        /// <typeparam name="T">The expando type.</typeparam>
        /// <param name="expando">The expando.</param>
        /// <param name="source">Source object to be merged into the expando.</param>
        /// <returns>
        /// The target expando object.
        /// </returns>
        public static T Merge<T>(this T expando, object? source)
            where T : IExpando
        {
            if (expando == null || source == null || (object)expando == source)
            {
                return expando;
            }

            // if the source is an enumeration of key-value pairs (dictionaries), then merge the dictionary.
            var itemType = source.GetType().TryGetEnumerableItemType();
            if (itemType != null && itemType.IsGenericType)
            {
                var genericItemTypeDefinition = itemType.GetGenericTypeDefinition();
                if (genericItemTypeDefinition == typeof(KeyValuePair<,>)
                    && itemType.GenericTypeArguments[0] == typeof(string))
                {
                    var keyProperty = itemType.GetProperty(nameof(KeyValuePair<string, object>.Key), BindingFlags.Instance | BindingFlags.Public);
                    var valueProperty = itemType.GetProperty(nameof(KeyValuePair<string, object>.Value), BindingFlags.Instance | BindingFlags.Public);
                    foreach (var kv in ((IEnumerable)source).Cast<object>())
                    {
                        var key = (string)keyProperty.GetValue(kv);
                        if (!key.IsPrivate())
                        {
                            var value = valueProperty.GetValue(kv);
                            expando[key] = value;
                        }
                    }

                    return expando;
                }
            }

            // for common objects, merge the properties.
            foreach (var kv in source.ToExpando().ToDictionary())
            {
                if (!kv.Key.IsPrivate())
                {
                    expando[kv.Key] = kv.Value;
                }
            }

            return expando;
        }

        /// <summary>
        /// An IExpando extension method that gets a member value using lax rules.
        /// </summary>
        /// <remarks>
        /// The member name may be either Pascal or camel case, and in case it is a string
        /// it is tried to be parsed.
        /// </remarks>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expando">The expando to act on.</param>
        /// <param name="member">The member.</param>
        /// <param name="defaultValue">Optional. The default value.</param>
        /// <returns>
        /// The lax value.
        /// </returns>
        public static T GetLaxValue<T>(this IIndexable expando, string member, T defaultValue = default)
        {
            Requires.NotNull(expando, nameof(expando));

            var pascalName = member.ToPascalCase();
            var value = expando[pascalName];
            if (value == null)
            {
                var camelName = member.ToCamelCase();
                value = expando[camelName];
            }

            if (value == null)
            {
                return defaultValue;
            }

            if (value is T typedValue)
            {
                return typedValue;
            }

            if (value is string stringValue)
            {
                if (Parsers.TryGetValue(typeof(T), out var parser))
                {
                    return (T)(parser(stringValue) ?? defaultValue);
                }
            }

            return defaultValue;
        }
    }
}