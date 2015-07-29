// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeDynamicMethod.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IDynamicProperty" /> for runtime properties.
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
    /// Implementation of <see cref="IDynamicProperty"/> for runtime properties.
    /// </summary>
    public class RuntimeDynamicMethod : IDynamicMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeDynamicMethod"/> class.
        /// </summary>
        /// <param name="methodInfo">
        /// The method information.
        /// </param>
        internal RuntimeDynamicMethod(MethodInfo methodInfo)
        {
            Contract.Requires(methodInfo != null);

            this.MethodInfo = methodInfo;
        }

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
    }
}
