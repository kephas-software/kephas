// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMethodInfo.cs" company="Kephas Software SRL">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Implementation of <see cref="IRuntimeMethodInfo"/> for runtime methods.
    /// </summary>
    public sealed class RuntimeMethodInfo : RuntimeElementInfoBase, IRuntimeMethodInfo
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private IDictionary<string, IRuntimeParameterInfo>? parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeMethodInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="logger">The logger.</param>
        internal RuntimeMethodInfo(IRuntimeTypeRegistry typeRegistry, MethodInfo methodInfo, ILogger? logger = null)
            : base(typeRegistry, logger)
        {
            Requires.NotNull(methodInfo, nameof(methodInfo));

            this.MethodInfo = methodInfo;
            this.Name = methodInfo.Name;
            this.FullName = methodInfo.DeclaringType?.FullName + "." + methodInfo.Name;
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
        /// Gets the runtime parameters.
        /// </summary>
        /// <value>
        /// The runtime parameters.
        /// </value>
        public IDictionary<string, IRuntimeParameterInfo> Parameters => this.GetParameters();

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        IEnumerable<IParameterInfo> IOperationInfo.Parameters => this.Parameters.Values;

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo? DeclaringContainer => this.TypeRegistry.GetTypeInfo(this.MethodInfo.DeclaringType);

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this method is static.
        /// </summary>
        /// <value>
        /// True if this method is static, false if not.
        /// </value>
        public bool IsStatic => this.MethodInfo.IsStatic;

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo IOperationInfo.ReturnType => this.TypeRegistry.GetTypeInfo(this.MethodInfo.ReturnType);

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public IRuntimeTypeInfo ReturnType => this.TypeRegistry.GetTypeInfo(this.MethodInfo.ReturnType);

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.MethodInfo;

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public object? Invoke(object? instance, IEnumerable<object?> args)
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
        public bool TryInvoke(object? instance, IEnumerable<object?> args, out object? result)
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name}({string.Join(", ", this.Parameters.Values.Select(p => p.ToString()))}): {this.ReturnType.FullName}";
        }

        /// <summary>
        /// Creates parameter infos.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>
        /// The new parameter infos.
        /// </returns>
        private IDictionary<string, IRuntimeParameterInfo> CreateParameterInfos(MethodInfo methodInfo)
        {
            var runtimeParameterInfos = new Dictionary<string, IRuntimeParameterInfo>();
            var parameterInfos = methodInfo.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                var runtimeParameterInfo = new RuntimeParameterInfo(this.TypeRegistry, parameterInfo, this, this.Logger);
                var parameterName = runtimeParameterInfo.Name;
                runtimeParameterInfos.Add(parameterName, runtimeParameterInfo);
            }

            return new ReadOnlyDictionary<string, IRuntimeParameterInfo>(runtimeParameterInfos);
        }

        /// <summary>
        /// Gets the parameter infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The parameters.
        /// </returns>
        private IDictionary<string, IRuntimeParameterInfo> GetParameters()
        {
            return this.parameters ??= this.CreateParameterInfos(this.MethodInfo);
        }
    }
}
