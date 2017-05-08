// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IRuntimePropertyInfo" /> for runtime properties.
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
    /// Implementation of <see cref="IRuntimePropertyInfo" /> for runtime properties.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    public sealed class RuntimePropertyInfo<T, TMember> : Expando, IRuntimePropertyInfo
    {
        /// <summary>
        /// The runtime type of <see cref="RuntimePropertyInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimePropertyInfo = new RuntimeTypeInfo(typeof(RuntimePropertyInfo<T, TMember>));

        /// <summary>
        /// The getter.
        /// </summary>
        private Func<T, TMember> getter;

        /// <summary>
        /// The setter.
        /// </summary>
        private Action<T, TMember> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimePropertyInfo{T,TMember}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        internal RuntimePropertyInfo(PropertyInfo propertyInfo)
            : base(isThreadSafe: true)
        {
            this.PropertyInfo = propertyInfo;
            this.Name = propertyInfo.Name;
            this.FullName = propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
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
        public IEnumerable<object> Annotations => this.PropertyInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.DeclaringType);

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public IRuntimeTypeInfo PropertyType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        ITypeInfo IPropertyInfo.PropertyType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public bool CanWrite => this.PropertyInfo.CanWrite;

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public bool CanRead => this.PropertyInfo.CanRead;

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo() => this.PropertyInfo;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.MemberAccessException">Property value cannot be set.</exception>
        public void SetValue(object obj, object value)
        {
            var setDelegate = this.GetMemberSetDelegate();
            if (setDelegate == null)
            {
                throw new MemberAccessException($"Value of property {this.PropertyInfo.Name} in {typeof(T)} cannot be set.");
            }

            setDelegate((T)obj, (TMember)value);
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
            var getDelegate = this.GetMemberGetDelegate();
            if (getDelegate == null)
            {
                throw new MemberAccessException($"Value of property {this.PropertyInfo.Name} in {typeof(T)} cannot be get.");
            }

            return getDelegate((T)obj);
        }

        /// <summary>
        /// Gets the <see cref="ITypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimePropertyInfo;
        }

        /// <summary>
        /// Gets the member get delegate.
        /// </summary>
        /// <returns>
        /// The member get delegate.
        /// </returns>
        private Func<T, TMember> GetMemberGetDelegate()
        {
            if (this.getter != null)
            {
                return this.getter;
            }

            var mi = this.PropertyInfo.GetMethod;
            if (mi != null && mi.IsPublic)
            {
                return this.getter = (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>));
            }

            return null;
        }

        /// <summary>
        /// Gets the member set delegate.
        /// </summary>
        /// <returns>
        /// The member set delegate.
        /// </returns>
        private Action<T, TMember> GetMemberSetDelegate()
        {
            if (this.setter != null)
            {
                return this.setter;
            }

            var mi = this.PropertyInfo.SetMethod;
            if (mi != null && mi.IsPublic)
            {
                return this.setter = (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>));
            }

            return null;
        }
    }
}
