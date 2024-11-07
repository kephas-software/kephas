// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.Reflection;

namespace Kephas.Dynamic;

/// <summary>
/// Extension methods for <see cref="IExpando"/>.
/// </summary>
public static class ExpandoExtensions
{
    /// <summary>
    /// Merges the indicated options into the context.
    /// </summary>
    /// <typeparam name="T">Type of the context.</typeparam>
    /// <typeparam name="TDynamic">Type of the expando contract.</typeparam>
    /// <param name="expando">The expando.</param>
    /// <param name="optionsConfig">The options configuration.</param>
    /// <returns>
    /// This <paramref name="expando"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Merge<T, TDynamic>(this T expando, Action<TDynamic>? optionsConfig)
        where T : class, TDynamic
        where TDynamic : IDynamic
    {
        expando = expando ?? throw new ArgumentNullException(nameof(expando));

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
        where T : class, IDynamic
    {
        expando = expando ?? throw new ArgumentNullException(nameof(expando));

        if (source == null || ReferenceEquals(expando, source))
        {
            return expando;
        }

        // if the source is an enumeration of key-value pairs (dictionaries), then merge the dictionary.
        var itemType = source.GetType().TryGetEnumerableItemType();
        if (itemType is { IsGenericType: true })
        {
            var genericItemTypeDefinition = itemType.GetGenericTypeDefinition();
            if (genericItemTypeDefinition == typeof(KeyValuePair<,>)
                && itemType.GenericTypeArguments[0] == typeof(string))
            {
                var keyProperty = itemType.GetProperty(nameof(KeyValuePair<string, object>.Key), BindingFlags.Instance | BindingFlags.Public)!;
                var valueProperty = itemType.GetProperty(nameof(KeyValuePair<string, object>.Value), BindingFlags.Instance | BindingFlags.Public)!;
                foreach (var kv in ((IEnumerable)source).Cast<object>())
                {
                    var key = (string)keyProperty.GetValue(kv)!;
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
        foreach (var kv in source.ToDictionary())
        {
            if (!kv.Key.IsPrivate())
            {
                expando[kv.Key] = kv.Value;
            }
        }

        return expando;
    }

    private static Type? TryGetEnumerableItemType(this Type type)
    {
        var enumerableGenericType = typeof(IEnumerable<>);

        bool IsRequestedEnumerable(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == enumerableGenericType;
        }

        var enumerableType = IsRequestedEnumerable(type)
            ? type
            : type.GetInterfaces().SingleOrDefault(IsRequestedEnumerable);

        return enumerableType?.GenericTypeArguments[0];
    }
}