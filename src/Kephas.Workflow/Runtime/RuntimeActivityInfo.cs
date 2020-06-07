// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeActivityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime activity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Runtime.AttributedModel;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Information about the runtime activity.
    /// </summary>
    public class RuntimeActivityInfo : RuntimeTypeInfo, IActivityInfo
    {
        private static readonly MethodInfo ExecuteAsyncMethodInfo =
            ReflectionHelper.GetMethodOf(_ => ((RuntimeActivityInfo)null).ExecuteAsync(null, null, null, null, default));

        private static readonly IDictionary<string, PropertyInfo> ActivityProperties =
            typeof(ActivityBase).GetProperties().ToDictionary(p => p.Name, p => p);

        private IDictionary<string, IRuntimeParameterInfo>? parameters;

        private ITypeInfo? returnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeActivityInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        protected internal RuntimeActivityInfo(IRuntimeTypeRegistry typeRegistry, Type type)
            : base(typeRegistry, type)
        {
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo ReturnType => this.returnType ??= this.ComputeReturnType();

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters => this.GetParameters();

        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        public virtual async Task<object?> ExecuteAsync(
            IActivity activity,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            activity.Target = target;
            activity.Arguments = arguments;
            activity.Context = context;

            if (activity is IOperation operation)
            {
                return await operation.ExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }

#if NETSTANDARD2_1
            // TODO localization
            throw new NotImplementedException($"Implement the {nameof(IOperation)} in the activity of type '{activity?.GetType()}', or provide a specialized type info.");
#else
            if (activity is IAsyncOperation asyncOperation)
            {
                return await asyncOperation.ExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }

            // TODO localization
            throw new NotImplementedException($"Either implement the {nameof(IOperation)} or {nameof(IAsyncOperation)} in the activity of type '{activity?.GetType()}', or provide a specialized type info.");
#endif
        }

        /// <summary>
        /// Executes the given operation.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// An object.
        /// </returns>
        object? IOperationInfo.Invoke(object? activity, IEnumerable<object?> args)
        {
            if (!(activity is IActivity))
            {
                throw new WorkflowException($"Expected activity '{activity}' to invoke, instead received {activity?.GetType()}.");
            }

            var argsList = new List<object?> { activity };
            argsList.AddRange(args);
            var target = argsList[0];
            var arguments = (IExpando?)argsList[1];
            var context = (IActivityContext)argsList[2];
            var cancellationToken = (CancellationToken)argsList[3];

            return ExecuteAsyncMethodInfo.Call(this, argsList.ToArray());
        }

        /// <summary>
        /// Creates the properties.
        /// </summary>
        /// <param name="type">The container type.</param>
        /// <param name="criteria">Optional. The criteria.</param>
        /// <returns>
        /// A dictionary of properties.
        /// </returns>
        protected override IDictionary<string, IRuntimePropertyInfo> CreatePropertyInfos(Type type, Func<PropertyInfo, bool>? criteria = null)
        {
            return base.CreatePropertyInfos(type, p => ActivityProperties.ContainsKey(p.Name));
        }

        /// <summary>
        /// Creates the member infos.
        /// </summary>
        /// <param name="membersConfig">Optional. The members configuration.</param>
        /// <returns>
        /// The new member infos.
        /// </returns>
        protected override IDictionary<string, IRuntimeElementInfo> CreateMemberInfos(Action<IDictionary<string, IRuntimeElementInfo>>? membersConfig = null)
        {
            void AddParameters(IDictionary<string, IRuntimeElementInfo> m)
            {
                this.Parameters.ForEach(p => m.Add(p.Name, (IRuntimeParameterInfo)p));

                membersConfig?.Invoke(m);
            }

            return base.CreateMemberInfos(AddParameters);
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="type">The container type.</param>
        /// <returns>
        /// A dictionary of parameters.
        /// </returns>
        protected virtual IDictionary<string, IRuntimeParameterInfo> CreateParameterInfos(Type type)
        {
            var memberTypeGetter = (Func<PropertyInfo, Type>)(prop => typeof(RuntimeActivityParameterInfo));

            var runtimeMembers = type.GetRuntimeProperties()
                .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic && p.GetMethod.IsPublic
                            && p.GetIndexParameters().Length == 0
                            && !ActivityProperties.ContainsKey(p.Name));

            return this.CreateMembers<PropertyInfo, IRuntimeParameterInfo>(type, runtimeMembers, memberTypeGetter);
        }

        /// <summary>
        /// Calculates the return type.
        /// </summary>
        /// <returns>
        /// The calculated return type.
        /// </returns>
        private ITypeInfo ComputeReturnType()
        {
            var returnTypeAttr = this.Type.GetCustomAttribute<ReturnTypeAttribute>();
            return this.TypeRegistry.GetRuntimeType(returnTypeAttr?.Value ?? typeof(void));
        }

        /// <summary>
        /// Gets the parameters collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the parameters in this collection.
        /// </returns>
        private IEnumerable<IParameterInfo> GetParameters()
        {
            return (this.parameters ??= this.CreateParameterInfos(this.Type)).Values;
        }
    }
}