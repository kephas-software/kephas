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
        private ITypeInfo? valueType;
        private string? valueTypeName;

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>
        /// The type of the parameter.
        /// </value>
        public ITypeInfo ValueType
        {
            get => this.valueType ??= this.TryGetType(this.valueTypeName);
            set => this.valueType = value;
        }

        /// <summary>
        /// Gets or sets the type name of the parameter.
        /// </summary>
        public string? ValueTypeName
        {
            get => this.valueTypeName ?? this.valueType?.FullName;
            set
            {
                this.valueTypeName = value;
                this.valueType = null;
            }
        }

        /// <summary>
        /// Gets or sets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position in the parameter's list.
        /// </value>
        public new int Position => base.Position;

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        public bool IsIn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        public bool IsOut { get; set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object? obj, object? value)
        {
            Requires.NotNull(obj, nameof(obj));

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
        public object? GetValue(object? obj)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IExpando expando)
            {
                return expando[this.Name];
            }

            return obj?.GetPropertyValue(this.Name);
        }
    }
}