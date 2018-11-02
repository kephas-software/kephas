// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic parameter information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Dynamic parameter information.
    /// </summary>
    public class DynamicParameterInfo : DynamicElementInfo, IParameterInfo
    {
        /// <summary>
        /// Gets or sets the parameter value type.
        /// </summary>
        /// <value>
        /// The parameter value type.
        /// </value>
        public ITypeInfo ValueType { get; protected internal set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
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
        public object GetValue(object obj)
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