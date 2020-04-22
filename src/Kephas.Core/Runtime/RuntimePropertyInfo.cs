// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Logging;
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
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. The logger.</param>
        internal RuntimePropertyInfo(PropertyInfo propertyInfo, int position = -1, ILogger logger = null)
            : base(isThreadSafe: true)
        {
            this.PropertyInfo = propertyInfo;
            this.Logger = logger;
            this.Name = propertyInfo.Name;
            this.FullName = propertyInfo.DeclaringType?.FullName + "." + propertyInfo.Name;
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
        /// Gets a value indicating whether this property is static.
        /// </summary>
        /// <value>
        /// True if this property is static, false if not.
        /// </value>
        public bool IsStatic => (this.PropertyInfo.GetMethod?.IsStatic ?? false) || (this.PropertyInfo.SetMethod?.IsStatic ?? false);

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public IRuntimeTypeInfo ValueType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        ITypeInfo IValueElementInfo.ValueType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanWrite => this.PropertyInfo.CanWrite;

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanRead => this.PropertyInfo.CanRead;

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
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.PropertyInfo;

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="MemberAccessException">Property value cannot be set.</exception>
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
            return this.PropertyInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.PropertyInfo.Name}: {this.PropertyInfo.PropertyType.FullName}";
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
        /// True if getter computed.
        /// </summary>
        private bool getterComputed = false;

        /// <summary>
        /// The getter.
        /// </summary>
        private Func<T, TMember> getter;

        /// <summary>
        /// True if setter computed.
        /// </summary>
        private bool setterComputed = false;

        /// <summary>
        /// The setter.
        /// </summary>
        private Action<T, TMember> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimePropertyInfo{T,TMember}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. The logger.</param>
        internal RuntimePropertyInfo(PropertyInfo propertyInfo, int position = -1, ILogger logger = null)
            : base(propertyInfo, position, logger)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public override bool CanRead => this.Getter != null && base.CanRead;

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public override bool CanWrite => this.Setter != null && base.CanWrite;

        /// <summary>
        /// Gets the getter.
        /// </summary>
        private Func<T, TMember> Getter
        {
            get
            {
                if (this.getterComputed)
                {
                    return this.getter;
                }

                this.getter = this.ComputeGetter();
                this.getterComputed = true;
                return this.getter;
            }
        }

        /// <summary>
        /// Gets the setter.
        /// </summary>
        private Action<T, TMember> Setter
        {
            get
            {
                if (this.setterComputed)
                {
                    return this.setter;
                }

                this.setter = this.ComputeSetter();
                this.setterComputed = true;
                return this.setter;
            }
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="MemberAccessException">Property value cannot be set.</exception>
        public override void SetValue(object obj, object value)
        {
            var setDelegate = this.Setter;
            if (setDelegate == null)
            {
                throw new MemberAccessException(string.Format(Strings.RuntimePropertyInfo_SetValue_Exception, this.PropertyInfo.Name, typeof(T)));
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
            var getDelegate = this.Getter;
            if (getDelegate == null)
            {
                throw new MemberAccessException(string.Format(Strings.RuntimePropertyInfo_GetValue_Exception, this.PropertyInfo.Name, typeof(T)));
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
        /// Computes the member get delegate.
        /// </summary>
        /// <returns>
        /// The member get delegate.
        /// </returns>
        private Func<T, TMember> ComputeGetter()
        {
            var mi = this.PropertyInfo.GetMethod;
            if (mi != null && mi.IsPublic)
            {
                try
                {
                    if (mi.IsSecurityTransparent)
                    {
                        return obj => (TMember)mi.Invoke(obj, EmptyArgs);
                    }

                    return (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>));
                }
                catch (ArgumentException ex)
                {
                    if (this.Logger.IsTraceEnabled())
                    {
                        this.Logger.Trace(ex, "Cannot compute getter delegate for {typeName}.{methodName}, falling back to reflection.", mi.DeclaringType, mi.Name);
                    }

                    return obj => (TMember)mi.Invoke(obj, EmptyArgs);
                }
            }

            return null;
        }

        /// <summary>
        /// Computes the member set delegate.
        /// </summary>
        /// <returns>
        /// The member set delegate.
        /// </returns>
        private Action<T, TMember> ComputeSetter()
        {
            var mi = this.PropertyInfo.SetMethod;
            if (mi != null && mi.IsPublic)
            {
                try
                {
                    if (mi.IsSecurityTransparent)
                    {
                        return (obj, v) => mi.Invoke(obj, new object[] { v });
                    }

                    return (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>));
                }
                catch (ArgumentException ex)
                {
                    if (this.Logger.IsTraceEnabled())
                    {
                        this.Logger.Trace(ex, "Cannot compute setter delegate for {typeName}.{methodName}, falling back to reflection.", mi.DeclaringType, mi.Name);
                    }

                    return (obj, v) => mi.Invoke(obj, new object[] { v });
                }
            }

            return null;
        }
    }
}
