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

            var objectTypeAccessor = obj.GetType().AsDynamicTypeInfo();
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

            var objectTypeAccessor = obj.GetType().AsDynamicTypeInfo();
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

            var dynamicType = obj.GetType().AsDynamicTypeInfo();
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
            var dynamicType = obj?.GetType().AsDynamicTypeInfo();
            return dynamicType?.TryGetValue(obj, propertyName);
        }

        /// <summary>
        /// Gets a dynamic type information out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic type information for the provided object.</returns>
        public static IDynamicTypeInfo GetDynamicTypeInfo(this object obj)
        {
            return obj?.GetType().AsDynamicTypeInfo();
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

        /// <summary>
        /// Gets the most specific type information out of the provided instance.
        /// If the object implements <see cref="IInstance"/>, then it returns
        /// the <see cref="ITypeInfo"/> provided by it, otherwise it returns the <see cref="IDynamicTypeInfo"/>
        /// of its runtime type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A type information for the provided object.</returns>
        public static ITypeInfo GetTypeInfo(this object obj)
        {
            var typeInfo = (obj as IInstance)?.GetTypeInfo();
            return typeInfo ?? obj?.GetType().AsDynamicTypeInfo();
        }
    }
}
