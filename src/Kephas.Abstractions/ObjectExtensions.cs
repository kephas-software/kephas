// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dynamic extension methods for objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Dynamic extension methods for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly MethodInfo ToTypedExpandoMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ToTypedExpando<string>(null!));

        /// <summary>
        /// Gets a dynamic object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is a dynamic object, or a dynamic wrapper over it.</returns>
        public static dynamic ToDynamicObject(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj switch
            {
                IDynamicMetaObjectProvider => obj,
                _ => obj.ToExpando()
            };
        }

        /// <summary>
        /// Gets an <see cref="IExpandoBase"/> object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is an <see cref="IExpandoBase"/>, or a dynamic wrapper over it.</returns>
        public static IExpandoBase ToExpando(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj switch
            {
                IExpandoBase expando => expando,
                IDictionary<string, object?> objectDictionary => new DictionaryExpando<object?>(objectDictionary),
                _ => ToExpandoCore(obj)
            };
        }

        /// <summary>
        /// Converts the provided instance to a dictionary.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is a dictionary, or a dictionary containing its content.</returns>
        public static IDictionary<string, object?> ToDictionary(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj switch
            {
                IDictionary<string, object?> dictionary => dictionary,
                IExpandoBase expando => expando.ToDictionary(),
                _ => obj.ToExpando().ToDictionary()
            };
        }

        /// <summary>
        /// Gets an <see cref="IDynamic"/> object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is an <see cref="IDynamic"/>, or a dynamic wrapper over it.</returns>
        public static IDynamic ToDynamic(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj switch
            {
                IDynamic dyn => dyn,
                _ => obj.ToExpando()
            };
        }

        private static IExpandoBase ToExpandoCore(object obj)
        {
            var keyItemTypePair = obj.GetType().TryGetDictionaryKeyItemType();
            if (keyItemTypePair is null || keyItemTypePair.Value.keyType != typeof(string))
            {
                return new ObjectExpando(obj);
            }

            var toTypedExpando = ToTypedExpandoMethod.MakeGenericMethod(keyItemTypePair.Value.itemType);
            return (IExpandoBase)toTypedExpando.Call(null, obj)!;
        }

        private static IExpandoBase ToTypedExpando<T>(IDictionary<string, T> dictionary) =>
            new DictionaryExpando<T>(dictionary);
    }
}
