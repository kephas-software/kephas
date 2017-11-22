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
    using Kephas.Resources;

    /// <summary>
    /// Implementation of <see cref="IRuntimePropertyInfo" /> for runtime properties.
    /// </summary>
    public class RuntimePropertyInfo : Expando, IRuntimePropertyInfo
    {
        /// <summary>
        /// The runtime type of <see cref="RuntimePropertyInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimePropertyInfo = new RuntimeTypeInfo(typeof(RuntimePropertyInfo));

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimePropertyInfo"/> class.
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
        /// <exception cref="MemberAccessException">Property value cannot be set.</exception>
        public virtual void SetValue(object obj, object value)
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
        public virtual object GetValue(object obj)
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
            return this.PropertyInfo.GetCustomAttributes<TAttribute>(inherit: true);
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
    }

    /// <summary>
    /// Implementation of <see cref="IRuntimePropertyInfo" /> for typed runtime properties.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    public sealed class RuntimePropertyInfo<T, TMember> : RuntimePropertyInfo
    {
        /// <summary>
        /// The empty arguments.
        /// </summary>
        private static readonly object[] EmptyArgs = new object[0];

        /// <summary>
        /// The runtime type of <see cref="RuntimePropertyInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfGenericRuntimePropertyInfo = new RuntimeTypeInfo(typeof(RuntimePropertyInfo<T, TMember>));

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
            : base(propertyInfo)
        {
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="MemberAccessException">Property value cannot be set.</exception>
        public override void SetValue(object obj, object value)
        {
            var setDelegate = this.GetMemberSetDelegate();
            if (setDelegate == null)
            {
                throw new MemberAccessException(String.Format(Strings.RuntimePropertyInfo_SetValue_Exception, this.PropertyInfo.Name, typeof(T)));
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
        public override object GetValue(object obj)
        {
            var getDelegate = this.GetMemberGetDelegate();
            if (getDelegate == null)
            {
                throw new MemberAccessException(String.Format(Strings.RuntimePropertyInfo_GetValue_Exception, this.PropertyInfo.Name, typeof(T)));
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
            return RuntimeTypeInfoOfGenericRuntimePropertyInfo;
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
                try
                {
                    return this.getter = (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>));
                }
                catch (ArgumentException)
                {
                    return this.getter = obj => (TMember)mi.Invoke(obj, EmptyArgs);
                }
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
                try
                {
                    return this.setter = (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>));
                }
                catch (ArgumentException)
                {
                    return this.setter = (obj, v) => mi.Invoke(obj, new object[] { v });
                }
            }

            return null;
        }
    }
}
