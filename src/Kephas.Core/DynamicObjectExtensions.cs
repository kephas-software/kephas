// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicObjectExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dynamic extension methods for objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Diagnostics.Contracts;
    using System.Dynamic;

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
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            Contract.Requires(obj != null);

            var objectTypeAccessor = obj.GetType().AsRuntimeTypeInfo();
            objectTypeAccessor.SetValue(obj, propertyName, value);
        }

        /// <summary>
        /// Dynamically sets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        /// <remarks>If the object passed is <c>null</c>, then <c>false</c> is returned.</remarks>
        public static bool TrySetPropertyValue(this object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                return false;
            }

            var objectTypeAccessor = obj.GetType().AsRuntimeTypeInfo();
            return objectTypeAccessor.TrySetValue(obj, propertyName, value);
        }

        /// <summary>
        /// Dynamically gets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value.</returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            Contract.Requires(obj != null);

            var dynamicType = obj.GetType().AsRuntimeTypeInfo();
            return dynamicType.GetValue(obj, propertyName);
        }

        /// <summary>
        /// Dynamically gets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value.</returns>
        /// <remarks>If the object passed is <c>null</c>, then <see cref="Undefined.Value"/> is returned.</remarks>
        public static object TryGetPropertyValue(this object obj, string propertyName)
        {
            var dynamicType = obj?.GetType().AsRuntimeTypeInfo();
            return dynamicType?.TryGetValue(obj, propertyName);
        }

        /// <summary>
        /// Gets a runtime type information out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic type information for the provided object.</returns>
        public static IRuntimeTypeInfo GetRuntimeTypeInfo(this object obj)
        {
            return obj?.GetType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Gets a dynamic object out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic object wrapping the provided object.</returns>
        public static dynamic ToDynamic(this object obj)
        {
            if (obj == null || obj is IDynamicMetaObjectProvider)
            {
                return obj;
            }

            return new Expando(obj);
        }
    }
}
