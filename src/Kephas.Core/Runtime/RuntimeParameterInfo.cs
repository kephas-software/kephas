// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime parameter information class.
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
    /// Information about the runtime parameter.
    /// </summary>
    public class RuntimeParameterInfo : RuntimeElementInfoBase, IRuntimeParameterInfo
    {
        /// <summary>
        /// The declaring container reference.
        /// </summary>
        private readonly WeakReference<IElementInfo> declaringContainerRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeParameterInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="parameterInfo">Information describing the parameter.</param>
        /// <param name="declaringContainer">The declaring element.</param>
        /// <param name="logger">Optional. The logger.</param>
        internal RuntimeParameterInfo(IRuntimeTypeRegistry typeRegistry, ParameterInfo parameterInfo, IElementInfo declaringContainer, ILogger? logger = null)
            : base(typeRegistry, logger)
        {
            this.ParameterInfo = parameterInfo;
            this.Name = parameterInfo.Name ?? $"__unnamed_{this.Position}";
            this.FullName = parameterInfo.Member.DeclaringType?.FullName + "." + parameterInfo.Member.Name + "." + parameterInfo.Name;
            this.declaringContainerRef = new WeakReference<IElementInfo>(declaringContainer);
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
        public IEnumerable<object> Annotations => this.ParameterInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer
        {
            get
            {
                if (this.declaringContainerRef.TryGetTarget(out var container))
                {
                    return container;
                }

                throw new ObjectDisposedException($"The container of {this.Name} is already disposed.");
            }
        }

        /// <summary>
        /// Gets information describing the parameter.
        /// </summary>
        /// <value>
        /// Information describing the parameter.
        /// </value>
        public ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position => this.ParameterInfo.Position;

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        public bool IsOptional => this.ParameterInfo.IsOptional;

        /// <summary>
        /// Gets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        public bool IsIn => this.ParameterInfo.IsIn;

        /// <summary>
        /// Gets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        public bool IsOut => this.ParameterInfo.IsOut;

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public IRuntimeTypeInfo ValueType => this.TypeRegistry.GetRuntimeType(this.ParameterInfo.ParameterType);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        ITypeInfo IValueElementInfo.ValueType => this.TypeRegistry.GetRuntimeType(this.ParameterInfo.ParameterType);

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(object? obj, object? value)
        {
            if (obj is IIndexable indexableObj)
            {
                indexableObj[this.Name] = value;
            }
            else
            {
                obj?.SetPropertyValue(this.Name, value);
            }
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public virtual object? GetValue(object? obj)
        {
            if (obj is IIndexable indexableObj)
            {
                return indexableObj[this.Name];
            }

            return obj?.GetPropertyValue(this.Name);
        }

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
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.ParameterInfo;

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
            return this.ParameterInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name}: {this.ParameterInfo.ParameterType.FullName}";
        }
    }
}