// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivityParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activity parameter information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Runtime.AttributedModel;

    /// <summary>
    /// Information about the runtime activity parameter.
    /// </summary>
    public class RuntimeActivityParameterInfo : Expando, IRuntimeParameterInfo
    {
        /// <summary>
        /// The runtime type of <see cref="RuntimeParameterInfo"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimeActivityParameterInfo = new RuntimeTypeInfo(typeof(RuntimeActivityParameterInfo));

        /// <summary>
        /// True if is optional, false if not.
        /// </summary>
        private bool? isOptional;

        /// <summary>
        /// True if is input, false if not.
        /// </summary>
        private bool? isIn;

        /// <summary>
        /// True if is output, false if not.
        /// </summary>
        private bool? isOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeActivityParameterInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property backing the parameter.</param>
        /// <param name="position">The position.</param>
        internal RuntimeActivityParameterInfo(PropertyInfo propertyInfo, int position)
            : base(isThreadSafe: true)
        {
            this.PropertyInfo = propertyInfo;
            this.Position = position;
            this.Name = propertyInfo.Name ?? $"__unnamed_{this.Position}";
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
        /// Gets information describing the parameter.
        /// </summary>
        /// <value>
        /// Information describing the parameter.
        /// </value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        public bool IsOptional =>
            this.isOptional ?? (this.isOptional = this.GetIsOptional()).Value;

        /// <summary>
        /// Gets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        public bool IsIn =>
            this.isIn ?? (this.isIn = this.GetIsIn()).Value;

        /// <summary>
        /// Gets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        public bool IsOut =>
            this.isOut ?? (this.isOut = this.GetIsOut()).Value;

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public IRuntimeTypeInfo ValueType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        ITypeInfo IValueElementInfo.ValueType => RuntimeTypeInfo.GetRuntimeType(this.PropertyInfo.PropertyType);

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(object obj, object value)
        {
            if (obj is IIndexable indexableObj)
            {
                indexableObj[this.Name] = value;
            }
            else
            {
                obj.SetPropertyValue(this.Name, value);
            }
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public virtual object GetValue(object obj)
        {
            if (obj is IIndexable indexableObj)
            {
                return indexableObj[this.Name];
            }

            return obj.GetPropertyValue(this.Name);
        }

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.PropertyInfo;

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
            return $"{this.Name}: {this.PropertyInfo.PropertyType.FullName}";
        }

        /// <summary>
        /// Gets the <see cref="ITypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeActivityParameterInfo;
        }

        /// <summary>
        /// Gets is optional.
        /// </summary>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool GetIsOptional()
        {
            if (this.PropertyInfo.GetCustomAttribute<RequiredAttribute>() != null)
            {
                return false;
            }

            var propType = this.PropertyInfo.PropertyType;
            if (propType.IsValueType && !propType.IsNullableType())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets is in.
        /// </summary>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool GetIsIn()
        {
            return this.PropertyInfo.GetCustomAttribute<InAttribute>() != null
                || this.PropertyInfo.GetCustomAttribute<OutAttribute>() == null;
        }

        /// <summary>
        /// Gets is out.
        /// </summary>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool GetIsOut()
        {
            return this.PropertyInfo.GetCustomAttribute<OutAttribute>() != null;
        }
    }
}