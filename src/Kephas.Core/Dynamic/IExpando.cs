// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpando.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamic objects allowing getting or setting
//   properties by their name through an indexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Contract for dynamic objects allowing getting or setting
    /// properties by their name through an indexer.
    /// </summary>
    public interface IExpando : IDynamicMetaObjectProvider, IIndexable, IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        bool HasMember(string memberName);

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the respective properties' values.
        /// </summary>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        IDictionary<string, object> ToDictionary();
    }

    /// <summary>
    /// Extension methods for <see cref="IExpando"/>.
    /// </summary>
    public static class ExpandoExtensions
    {
        /// <summary>
        /// Information describing the string type.
        /// </summary>
        private static readonly IRuntimeTypeInfo StringTypeInfo = typeof(string).AsRuntimeTypeInfo();

        /// <summary>
        /// Merges the source object properties into the expando.
        /// </summary>
        /// <remarks>
        /// Collections of key-value pairs (including dictionaries) are merged by their keys, provided the key has the type of string.
        /// </remarks>
        /// <typeparam name="T">The expando type.</typeparam>
        /// <param name="expando">The expando to act on.</param>
        /// <param name="source">Source object to be merged into the expando.</param>
        /// <returns>
        /// The target expando object.
        /// </returns>
        public static T Merge<T>(this T expando, object source)
            where T : IExpando
        {
            if (expando == null || source == null || (object)expando == source)
            {
                return expando;
            }

            // if the source is an enumeration of key-value pairs (dictionaries), then merge the dictionary.
            var itemType = source.GetType().TryGetEnumerableItemType()?.AsRuntimeTypeInfo();
            if (itemType != null && itemType.IsGenericType())
            {
                var genericItemTypeDefinition = itemType.TypeInfo.GetGenericTypeDefinition();
                if (genericItemTypeDefinition == typeof(KeyValuePair<,>)
                    && itemType.GenericTypeArguments[0] == StringTypeInfo)
                {
                    var keyProperty = itemType.Properties[nameof(KeyValuePair<string, object>.Key)];
                    var valueProperty = itemType.Properties[nameof(KeyValuePair<string, object>.Value)];
                    foreach (var kv in ((IEnumerable)source).Cast<object>())
                    {
                        var key = (string)keyProperty.GetValue(kv);
                        var value = valueProperty.GetValue(kv);
                        expando[key] = value;
                    }

                    return expando;
                }
            }

            // for common objects, merge the properties.
            foreach (var kv in source.ToExpando().ToDictionary())
            {
                expando[kv.Key] = kv.Value;
            }

            return expando;
        }
    }
}