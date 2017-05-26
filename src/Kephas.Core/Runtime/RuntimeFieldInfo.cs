// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeFieldInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Reflection;

    /// <summary>
    /// Implementation of <see cref="IRuntimeFieldInfo" /> for runtime fields.
    /// </summary>
    public class RuntimeFieldInfo : Expando, IRuntimeFieldInfo
    {
        /// <summary>
        /// The runtime type of <see cref="RuntimeFieldInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimeFieldInfo = new RuntimeTypeInfo(typeof(RuntimeFieldInfo));

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFieldInfo"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        internal RuntimeFieldInfo(FieldInfo fieldInfo)
            : base(isThreadSafe: true)
        {
            this.FieldInfo = fieldInfo;
            this.Name = fieldInfo.Name;
            this.FullName = fieldInfo.DeclaringType.FullName + "." + fieldInfo.Name;
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
        public IElementInfo DeclaringContainer => RuntimeTypeInfo.GetRuntimeType(this.FieldInfo.DeclaringType);

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public IRuntimeTypeInfo FieldType => RuntimeTypeInfo.GetRuntimeType(this.FieldInfo.FieldType);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        ITypeInfo IFieldInfo.FieldType => RuntimeTypeInfo.GetRuntimeType(this.FieldInfo.FieldType);

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo() => this.FieldInfo;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.MemberAccessException">Property value cannot be set.</exception>
        public void SetValue(object obj, object value)
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
        public object GetValue(object obj)
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
            return this.FieldInfo.GetCustomAttributes<TAttribute>();
        }

        /// <summary>
        /// Gets the <see cref="ITypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeFieldInfo;
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
        /// The runtime type of <see cref="RuntimeFieldInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfGenericRuntimeFieldInfo =
            new RuntimeTypeInfo(typeof(RuntimeFieldInfo<T, TMember>));

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeFieldInfo{T,TMember}"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field information.</param>
        internal RuntimeFieldInfo(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
        }

        /// <summary>
        /// Gets the <see cref="ITypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfGenericRuntimeFieldInfo;
        }
    }
}