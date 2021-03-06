// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTransitionMethodInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime method activity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Workflow.AttributedModel;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Information about the runtime method representing a transition.
    /// </summary>
    public sealed class RuntimeTransitionMethodInfo : RuntimeElementInfoBase, IRuntimeElementInfo, ITransitionInfo
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private IDictionary<string, IRuntimeParameterInfo>? parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTransitionMethodInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="methodInfo">The method information.</param>
        internal RuntimeTransitionMethodInfo(IRuntimeTypeRegistry typeRegistry, MethodInfo methodInfo)
            : base(typeRegistry)
        {
            Requires.NotNull(methodInfo, nameof(methodInfo));

            const string AsyncEnding = "Async";

            this.MethodInfo = methodInfo;
            this.Name = methodInfo.Name.EndsWith(AsyncEnding)
                ? methodInfo.Name.Substring(0, methodInfo.Name.Length - AsyncEnding.Length)
                : methodInfo.Name;
            this.FullName = methodInfo.DeclaringType?.FullName + "." + this.Name;

            var transitionAttr = methodInfo.GetCustomAttribute<TransitionAttribute>();
            if (transitionAttr == null)
            {
                this.To = new object();
            }
            else
            {
                this.From = transitionAttr.From;
                this.To = transitionAttr.To;
            }
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
        /// Gets the states from which the transitions starts.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        public IEnumerable<object> From { get; private set; } = Enumerable.Empty<object>();

        /// <summary>
        /// Gets the state to which the state machine is transitioned.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        public object To { get; private set; }

#if NETSTANDARD2_0
        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public IDisplayInfo? GetDisplayInfo() => ElementInfoHelper.GetDisplayInfo(this);
#endif

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.MethodInfo;

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
        /// Executes the given operation on a different thread, and waits for the result.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The execution result.
        /// </returns>
        public object? Invoke(object? instance, IEnumerable<object?> args)
        {
            return this.MethodInfo.Invoke(instance, args.ToArray());
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name}[({string.Join(", ", this.From)}) -> {this.To}]({string.Join(", ", this.Parameters.Values.Select(p => p.ToString()))}): {this.ReturnType.FullName}";
        }

        private IDictionary<string, IRuntimeParameterInfo> CreateParameterInfos(MethodInfo methodInfo)
        {
            var runtimeParameterInfos = new Dictionary<string, IRuntimeParameterInfo>();
            var parameterInfos = methodInfo.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                var runtimeParameterInfo = new RuntimeParameterInfo(this.TypeRegistry, parameterInfo, this);
                var parameterName = runtimeParameterInfo.Name;
                runtimeParameterInfos.Add(parameterName, runtimeParameterInfo);
            }

            return new ReadOnlyDictionary<string, IRuntimeParameterInfo>(runtimeParameterInfos);
        }

        private IDictionary<string, IRuntimeParameterInfo> GetParameters()
        {
            return this.parameters ??= this.CreateParameterInfos(this.MethodInfo);
        }
    }
}
