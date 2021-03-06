﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicObjectExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dynamic extension methods for objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Collections.Generic;
    using System.Dynamic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Dynamic extension methods for objects.
    /// </summary>
    public static class DynamicObjectExtensions
    {
        /// <summary>
        /// Dynamically sets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this object obj, string propertyName, object? value)
        {
            Requires.NotNull(obj, nameof(obj));

            var runtimeTypeInfo = obj.GetType().AsRuntimeTypeInfo();
            runtimeTypeInfo.SetValue(obj, propertyName, value);
        }

        /// <summary>
        /// Dynamically sets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        /// <remarks>If the object passed is <c>null</c>, then <c>false</c> is returned.</remarks>
        public static bool TrySetPropertyValue(this object? obj, string propertyName, object? value)
        {
            if (obj == null)
            {
                return false;
            }

            var runtimeTypeInfo = obj.GetType().AsRuntimeTypeInfo();
            return runtimeTypeInfo.TrySetValue(obj, propertyName, value);
        }

        /// <summary>
        /// Dynamically gets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value.</returns>
        public static object? GetPropertyValue(this object obj, string propertyName)
        {
            Requires.NotNull(obj, nameof(obj));

            var runtimeTypeInfo = obj.GetType().AsRuntimeTypeInfo();
            return runtimeTypeInfo.GetValue(obj, propertyName);
        }

        /// <summary>
        /// Dynamically gets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The property value.</param>
        /// <returns>
        /// A boolean value indicating whether the property is found.
        /// </returns>
        public static bool TryGetPropertyValue(this object obj, string propertyName, out object? value)
        {
            Requires.NotNull(obj, nameof(obj));

            var runtimeTypeInfo = obj.GetType().AsRuntimeTypeInfo();
            return runtimeTypeInfo.TryGetValue(obj, propertyName, out value);
        }

        /// <summary>
        /// Gets a runtime type information out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic type information for the provided object.</returns>
        public static IRuntimeTypeInfo GetRuntimeTypeInfo(this object obj)
        {
            Requires.NotNull(obj, nameof(obj));
            return obj.GetType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Gets a dynamic object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is a dynamic object, or a dynamic wrapper over it.</returns>
        public static dynamic ToDynamicObject(this object obj)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj switch
            {
                IDynamicMetaObjectProvider => obj,
                _ => new Expando(obj)
            };
        }

        /// <summary>
        /// Gets an <see cref="IExpandoBase"/> object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is an <see cref="IExpandoBase"/>, or a dynamic wrapper over it.</returns>
        public static IExpandoBase ToExpando(this object obj)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj switch
            {
                IExpandoBase expando => expando,
                _ => new Expando(obj)
            };
        }

        /// <summary>
        /// Converts the provided instance to a dictionary.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is a dictionary, or a dictionary containing its content.</returns>
        public static IDictionary<string, object?> ToDictionary(this object obj)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj switch
            {
                IDictionary<string, object?> dictionary => dictionary,
                IExpandoBase expando => expando.ToDictionary(),
                _ => new Expando(obj).ToDictionary()
            };
        }

        /// <summary>
        /// Gets an <see cref="IDynamic"/> object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The provided instance, if it is an <see cref="IDynamic"/>, or a dynamic wrapper over it.</returns>
        public static IDynamic ToDynamic(this object obj)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj switch
            {
                IDynamic dyn => dyn,
                _ => new Expando(obj)
            };
        }
    }
}
