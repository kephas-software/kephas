// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimePropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamically accessing a property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="PropertyInfo"/>.
    /// </summary>
    public interface IRuntimePropertyInfo : IPropertyInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        new IRuntimeTypeInfo PropertyType { get; }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        void SetValue(object obj, object value);

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        object GetValue(object obj);
    }
}
