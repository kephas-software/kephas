// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Implementation of <see cref="IRuntimeMethodInfo"/> for runtime methods.
    /// </summary>
    public sealed class RuntimeMethodInfo : Expando, IRuntimeMethodInfo
    {
        /// <summary>
        /// The <see cref="IRuntimeTypeInfo"/> of <see cref="RuntimeMethodInfo"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimeMethodInfo = new RuntimeTypeInfo(typeof(RuntimeMethodInfo));

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">
        /// The method information.
        /// </param>
        internal RuntimeMethodInfo(MethodInfo methodInfo)
            : base(isThreadSafe: true)
        {
            Requires.NotNull(methodInfo, nameof(methodInfo));

            this.MethodInfo = methodInfo;
            this.Name = methodInfo.Name;
            this.FullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
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
        public IEnumerable<object> Annotations => this.MethodInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer => RuntimeTypeInfo.GetRuntimeType(this.MethodInfo.DeclaringType);

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo IMethodInfo.ReturnType => RuntimeTypeInfo.GetRuntimeType(this.MethodInfo.ReturnType);

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public IRuntimeTypeInfo ReturnType => RuntimeTypeInfo.GetRuntimeType(this.MethodInfo.ReturnType);

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo() => this.MethodInfo;

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public object Invoke(object instance, IEnumerable<object> args)
        {
            return this.MethodInfo.Invoke(instance, args.ToArray());
        }

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The invocation result.</param>
        /// <returns>A boolean value indicating whether the invocation was successful or not.</returns>
        public bool TryInvoke(object instance, IEnumerable<object> args, out object result)
        {
            try
            {
                result = this.MethodInfo.Invoke(instance, args.ToArray());
                return true;
            }
            catch (Exception)
            {
                result = this.ReturnType.DefaultValue;
                return false;
            }
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
            return this.MethodInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuntimeTypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeMethodInfo;
        }
    }
}
