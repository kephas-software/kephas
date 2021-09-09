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
    using System.Threading;

    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;

    /// <summary>
    /// Implementation of <see cref="IRuntimePropertyInfo" /> for runtime properties.
    /// </summary>
    public class RuntimePropertyInfo : RuntimeElementInfoBase, IRuntimePropertyInfo
    {
        private static readonly MethodInfo ComputeGetterMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((RuntimePropertyInfo)null!).ComputeGetter<int, int>());

        private static readonly MethodInfo ComputeSetterMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((RuntimePropertyInfo)null!).ComputeSetter<int, int>());

        private readonly Lazy<Func<object?, object?>?> lazyGetter;
        private readonly Lazy<Action<object?, object?>?> lazySetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimePropertyInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimePropertyInfo(IRuntimeTypeRegistry typeRegistry, PropertyInfo propertyInfo, int position = -1, ILogger? logger = null)
            : base(typeRegistry, logger)
        {
            this.PropertyInfo = propertyInfo;
            this.Name = propertyInfo.Name;
            this.FullName = propertyInfo.DeclaringType?.FullName + "." + propertyInfo.Name;

            this.lazyGetter = new Lazy<Func<object?, object?>?>(this.ComputeGetter, LazyThreadSafetyMode.PublicationOnly);
            this.lazySetter = new Lazy<Action<object?, object?>?>(this.ComputeSetter, LazyThreadSafetyMode.PublicationOnly);
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
        public IElementInfo DeclaringContainer => this.TypeRegistry.GetTypeInfo(this.PropertyInfo.DeclaringType!);

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
        public IRuntimeTypeInfo ValueType => this.TypeRegistry.GetTypeInfo(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        ITypeInfo IValueElementInfo.ValueType => this.TypeRegistry.GetTypeInfo(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanWrite => this.lazySetter.Value != null && this.PropertyInfo.CanWrite;

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public virtual bool CanRead => this.lazyGetter.Value != null && this.PropertyInfo.CanRead;

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
            var setDelegate = this.lazySetter.Value;
            if (setDelegate == null)
            {
                throw new MemberAccessException(string.Format(Strings.RuntimePropertyInfo_SetValue_Exception, this.PropertyInfo.Name, this.PropertyInfo.DeclaringType));
            }

            setDelegate(obj, value);
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
            var getDelegate = this.lazyGetter.Value;
            if (getDelegate == null)
            {
                throw new MemberAccessException(string.Format(Strings.RuntimePropertyInfo_GetValue_Exception, this.PropertyInfo.Name, this.PropertyInfo.DeclaringType));
            }

            return getDelegate(obj);
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.PropertyInfo.Name}: {this.PropertyInfo.PropertyType.FullName}";
        }

        private Func<object?, object?>? ComputeGetter()
        {
            var propType = this.PropertyInfo.PropertyType;
            var declType = this.PropertyInfo.DeclaringType;
            if (propType.ContainsGenericParameters || declType!.ContainsGenericParameters)
            {
                return null;
            }

            var computeGetter = ComputeGetterMethod.MakeGenericMethod(declType, propType);
            return (Func<object?, object?>?)computeGetter.Call(this);
        }

        private Func<object?, object?>? ComputeGetter<T, TMember>()
        {
            var mi = this.PropertyInfo.GetMethod;
            if (mi == null || !mi.IsPublic)
            {
                return null;
            }

            try
            {
                if (mi.IsSecurityTransparent)
                {
                    return obj => mi.Invoke(obj, Array.Empty<object>());
                }

                var getter = (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>));
                return o => getter((T)o);
            }
            catch (ArgumentException ex)
            {
                if (this.Logger.IsTraceEnabled())
                {
                    this.Logger.Trace(ex, "Cannot compute getter delegate for {typeName}.{methodName}, falling back to reflection.", mi.DeclaringType, mi.Name);
                }

                return obj => mi.Invoke(obj, Array.Empty<object>());
            }
        }

        private Action<object?, object?>? ComputeSetter()
        {
            var propType = this.PropertyInfo.PropertyType;
            var declType = this.PropertyInfo.DeclaringType;
            if (propType.ContainsGenericParameters || declType!.ContainsGenericParameters)
            {
                return null;
            }

            var computeSetter = ComputeSetterMethod.MakeGenericMethod(propType, declType);
            return (Action<object?, object?>?)computeSetter.Call(this);
        }

        private Action<object?, object?>? ComputeSetter<T, TMember>()
        {
            var mi = this.PropertyInfo.SetMethod;
            if (mi == null || !mi.IsPublic)
            {
                return null;
            }

            try
            {
                if (mi.IsSecurityTransparent)
                {
                    return (obj, v) => mi.Invoke(obj, new object?[] { v });
                }

                var setter = (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>));
                return (o, v) => setter((T)o, (TMember)v);
            }
            catch (ArgumentException ex)
            {
                if (this.Logger.IsTraceEnabled())
                {
                    this.Logger.Trace(ex, "Cannot compute setter delegate for {typeName}.{methodName}, falling back to reflection.", mi.DeclaringType, mi.Name);
                }

                return (obj, v) => mi.Invoke(obj, new object?[] { v });
            }
        }
    }
}
