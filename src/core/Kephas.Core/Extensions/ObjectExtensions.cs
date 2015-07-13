// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions
{
    using System.Dynamic;

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Dynamically sets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                return;
            }

            var objectTypeAccessor = obj.GetType().GetDynamicType();
            objectTypeAccessor.SetValue(obj, propertyName, value);
        }

        /// <summary>
        /// Dynamically sets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        public static bool TrySetPropertyValue(this object obj, string propertyName, object value)
        {
            if (obj == null)
            {
                return false;
            }

            var objectTypeAccessor = obj.GetType().GetDynamicType();
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
            if (obj == null)
            {
                return null;
            }

            var dynamicType = obj.GetType().GetDynamicType();
            return dynamicType.GetValue(obj, propertyName);
        }

        /// <summary>
        /// Dynamically gets the property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value.</returns>
        public static object TryGetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
            {
                return Undefined.Value;
            }

            var dynamicType = obj.GetType().GetDynamicType();
            return dynamicType.TryGetValue(obj, propertyName);
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
