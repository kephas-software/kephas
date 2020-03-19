// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicObjectExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dynamic extension methods for objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas
{
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
        public static IRuntimeTypeInfo? GetRuntimeTypeInfo(this object? obj)
        {
            return obj?.GetType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Gets a dynamic object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic object wrapping the provided object.</returns>
        public static dynamic? ToDynamic(this object obj)
        {
            if (obj == null || obj is IDynamicMetaObjectProvider)
            {
                return obj;
            }

            return new Expando(obj);
        }

        /// <summary>
        /// Gets an <see cref="IExpando"/> object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>An <see cref="IExpando"/> wrapping the provided object. If the provided object is an expando, that object is returned.</returns>
        public static IExpando? ToExpando(this object? obj)
        {
            if (obj == null || obj is IExpando)
            {
                return (IExpando?)obj;
            }

            return new Expando(obj);
        }
    }
}
