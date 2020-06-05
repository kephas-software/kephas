// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeFieldInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime field information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Implementation of <see cref="IRuntimeFieldInfo" /> for runtime fields.
    /// </summary>
    public class RuntimeFieldInfo : RuntimeElementInfoBase, IRuntimeFieldInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFieldInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type serviceRegistry.</param>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. the logger.</param>
        internal RuntimeFieldInfo(IRuntimeTypeRegistry typeRegistry, FieldInfo fieldInfo, int position = -1, ILogger? logger = null)
            : base(typeRegistry, logger)
        {
            this.FieldInfo = fieldInfo;
            this.Name = fieldInfo.Name;
            this.FullName = fieldInfo.DeclaringType?.FullName + "." + fieldInfo.Name;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public string FullName { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.FieldInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer => this.TypeRegistry.GetRuntimeType(this.FieldInfo.DeclaringType);

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this field is static.
        /// </summary>
        /// <value>
        /// True if this field is static, false if not.
        /// </value>
        public bool IsStatic => this.FieldInfo.IsStatic;

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public IRuntimeTypeInfo ValueType => this.TypeRegistry.GetRuntimeType(this.FieldInfo.FieldType);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        ITypeInfo IValueElementInfo.ValueType => this.TypeRegistry.GetRuntimeType(this.FieldInfo.FieldType);

#if NETSTANDARD2_1
#else
        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public virtual IDisplayInfo? GetDisplayInfo() => ElementInfoHelper.GetDisplayInfo(this);
#endif

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.FieldInfo;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.MemberAccessException">Property value cannot be set.</exception>
        public virtual void SetValue(object? obj, object? value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        /// <exception cref="MemberAccessException">Property value cannot be get.</exception>
        public virtual object? GetValue(object? obj)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return this.FieldInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name}: {this.FieldInfo.FieldType.FullName}";
        }
    }

    /// <summary>
    /// Implementation of <see cref="IRuntimeFieldInfo" /> for runtime fields.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    public sealed class RuntimeFieldInfo<T, TMember> : RuntimeFieldInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFieldInfo{T,TMember}"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type serviceRegistry.</param>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. The logger.</param>
        internal RuntimeFieldInfo(IRuntimeTypeRegistry typeRegistry, FieldInfo fieldInfo, int position = -1, ILogger? logger = null)
            : base(typeRegistry, fieldInfo, position, logger)
        {
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.MemberAccessException">Property value cannot be set.</exception>
        public override void SetValue(object? obj, object? value)
        {
            this.FieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        /// <exception cref="MemberAccessException">Property value cannot be get.</exception>
        public override object? GetValue(object? obj)
        {
            return this.FieldInfo.GetValue(obj);
        }
    }
}