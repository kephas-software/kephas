// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicPropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic property information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Dynamic property information.
    /// </summary>
    public class DynamicPropertyInfo : DynamicElementInfo, IPropertyInfo
    {
        private ITypeInfo? valueType;
        private string? valueTypeName;

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public virtual ITypeInfo ValueType
        {
            get => this.valueType ??= this.TryGetType(this.valueTypeName);
            set => this.valueType = value;
        }

        /// <summary>
        /// Gets or sets the type name of the property.
        /// </summary>
        public virtual string? ValueTypeName
        {
            get => this.valueTypeName ?? this.valueType?.FullName;
            set
            {
                this.valueTypeName = value;
                this.valueType = null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanWrite { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanRead { get; set; } = true;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(object? obj, object? value)
        {
            Requires.NotNull(obj, nameof(obj));

            if (!this.CanWrite)
            {
                throw new InvalidOperationException($"Property '{this.Name}' is read-only.");
            }

            if (obj is IExpando expando)
            {
                expando[this.Name] = value;
            }

            obj?.SetPropertyValue(this.Name, value);
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public virtual object? GetValue(object? obj)
        {
            Requires.NotNull(obj, nameof(obj));

            if (!this.CanRead)
            {
                throw new InvalidOperationException($"Property '{this.Name}' is write-only.");
            }

            if (obj is IExpando expando)
            {
                return expando[this.Name];
            }

            return obj?.GetPropertyValue(this.Name);
        }
    }
}