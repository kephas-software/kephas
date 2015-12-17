// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicPropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IDynamicPropertyInfo" /> for runtime properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Implementation of <see cref="IDynamicPropertyInfo" /> for runtime properties.
    /// </summary>
    /// <typeparam name="T">The container type.</typeparam>
    /// <typeparam name="TMember">The member type.</typeparam>
    public sealed class DynamicPropertyInfo<T, TMember> : Expando, IDynamicPropertyInfo
    {
        /// <summary>
        /// The dynamic type of <see cref="DynamicPropertyInfo{T,TMember}"/>.
        /// </summary>
        private static readonly IDynamicTypeInfo DynamicTypeInfoOfDynamicPropertyInfo = new DynamicTypeInfo(typeof(DynamicPropertyInfo<T,TMember>));

        /// <summary>
        /// The getter.
        /// </summary>
        private Func<T, TMember> getter;

        /// <summary>
        /// The setter.
        /// </summary>
        private Action<T, TMember> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyInfo{T,TMember}"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property information.
        /// </param>
        internal DynamicPropertyInfo(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
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
        /// Gets the dynamic type used by the expando in the dynamic behavior.
        /// </summary>
        /// <returns>
        /// The dynamic type.
        /// </returns>
        protected override IDynamicTypeInfo GetDynamicTypeInfo()
        {
            return DynamicTypeInfoOfDynamicPropertyInfo;
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
                return (this.getter = (Func<T, TMember>)mi.CreateDelegate(typeof(Func<T, TMember>)));
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
                return (this.setter = (Action<T, TMember>)mi.CreateDelegate(typeof(Action<T, TMember>)));
            }

            return null;
        }
    }
}
