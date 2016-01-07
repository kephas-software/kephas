// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IDynamicPropertyInfo" /> for runtime properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Implementation of <see cref="IDynamicMethodInfo"/> for runtime methods.
    /// </summary>
    public sealed class DynamicMethodInfo : Expando, IDynamicMethodInfo
    {
        /// <summary>
        /// The <see cref="IDynamicTypeInfo"/> of <see cref="DynamicMethodInfo"/>.
        /// </summary>
        private static readonly IDynamicTypeInfo DynamicTypeInfoOfDynamicMethodInfo = new DynamicTypeInfo(typeof(DynamicMethodInfo));

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">
        /// The method information.
        /// </param>
        internal DynamicMethodInfo(MethodInfo methodInfo)
            : base(isThreadSafe: true)
        {
            Contract.Requires(methodInfo != null);

            this.MethodInfo = methodInfo;
            this.Name = methodInfo.Name;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.MethodInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

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
        protected override IDynamicTypeInfo GetDynamicTypeInfo()
        {
            return DynamicTypeInfoOfDynamicMethodInfo;
        }
    }
}
