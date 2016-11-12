// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMethodInfo.cs" company="Quartz Software SRL">
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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

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
            Contract.Requires(methodInfo != null);

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
        public ITypeInfo ReturnType => RuntimeTypeInfo.GetRuntimeType(this.MethodInfo.ReturnType);

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
        /// The try invoke.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public object TryInvoke(object instance, IEnumerable<object> args)
        {
            try
            {
                return this.MethodInfo.Invoke(instance, args.ToArray());
            }
            catch (Exception)
            {
                return Undefined.Value;
            }
        }

        /// <summary>
        /// Gets the dynamic type used by the expando in the dynamic behavior.
        /// </summary>
        /// <returns>
        /// The dynamic type.
        /// </returns>
        protected override IRuntimeTypeInfo GetRuntimeTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeMethodInfo;
        }
    }
}
