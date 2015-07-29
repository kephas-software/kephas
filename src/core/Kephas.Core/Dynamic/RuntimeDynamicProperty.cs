// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeDynamicProperty.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IDynamicProperty" /> for runtime properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Implementation of <see cref="IDynamicProperty" /> for runtime properties.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    public class RuntimeDynamicProperty<T, TMember> : IDynamicProperty
    {
        /// <summary>
        /// The getter.
        /// </summary>
        private readonly Func<T, TMember> getter;

        /// <summary>
        /// The setter.
        /// </summary>
        private readonly Action<T, TMember> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeDynamicProperty{T,TMember}"/> class. 
        /// </summary>
        /// <param name="propertyInfo">
        /// The property information.
        /// </param>
        internal RuntimeDynamicProperty(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;

            this.getter = this.GetMemberGetDelegate();
            this.setter = this.GetMemberSetDelegate();
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.MemberAccessException">Property value cannot be set.</exception>
        public void SetValue(object obj, object value)
        {
            if (this.setter == null)
            {
                throw new MemberAccessException($"Value of property {this.PropertyInfo.Name} in {typeof(T)} cannot be set.");
            }

            this.setter((T)obj, (TMember)value);
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        /// <exception cref="System.MemberAccessException">Property value cannot be get.</exception>
        public object GetValue(object obj)
        {
            if (this.getter == null)
            {
                throw new MemberAccessException($"Value of property {this.PropertyInfo.Name} in {typeof(T)} cannot be get.");
            }

            return this.getter((T)obj);
        }

        /// <summary>
        /// Gets the member get delegate.
        /// </summary>
        /// <returns>
        /// The member get delegate.
        /// </returns>
        private Func<T, TMember> GetMemberGetDelegate()
        {
            var mi = this.PropertyInfo.GetMethod;
            if (mi != null && mi.IsPublic)
            {
                return (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>));
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
            var mi = this.PropertyInfo.SetMethod;
            if (mi != null && mi.IsPublic)
            {
                return (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>));
            }

            return null;
        }
    }
}
