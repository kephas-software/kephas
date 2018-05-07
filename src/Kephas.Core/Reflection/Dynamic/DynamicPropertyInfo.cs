// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicPropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic property information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Dynamic property information.
    /// </summary>
    public class DynamicPropertyInfo : DynamicElementInfo, IPropertyInfo
    {
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public ITypeInfo ValueType { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public bool CanWrite { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public bool CanRead { get; protected internal set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(object obj, object value)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IExpando expando)
            {
                expando[this.Name] = value;
            }

            obj.SetPropertyValue(this.Name, value);
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public virtual object GetValue(object obj)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IExpando expando)
            {
                return expando[this.Name];
            }

            return obj.GetPropertyValue(this.Name);
        }
    }
}