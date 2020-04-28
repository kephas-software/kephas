// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity type class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using Kephas.Operations;
using Kephas.Threading.Tasks;

namespace Kephas.Workflow.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Classifier for activities.
    /// </summary>
    public class ActivityType : ClassifierBase<IActivityType>, IActivityType
    {
        private static readonly MethodInfo ExecuteAsyncMethodInfo =
            ReflectionHelper.GetMethodOf(_ => ((ActivityType)null).ExecuteAsync(null, null, null, null, default));
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityType"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public ActivityType(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets or sets the type of the return value.
        /// </summary>
        /// <value>
        /// The type of the return value.
        /// </value>
        public ITypeInfo? ReturnType { get; protected set; }

        /// <summary>
        /// Gets the parameters for controlling the activity.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IEnumerable<IParameterInfo> Parameters => this.Members.OfType<IParameterInfo>();

        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<object> IAggregatedElementInfo.Parts => this.Parts;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.Annotations;

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.Members;

        /// <summary>
        /// Gets the enumeration of properties.
        /// </summary>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Properties;

        /// <summary>
        /// Executes the asynchronous operation.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the execution output.
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
        /// Executes the given operation and returns the result.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The execution result.
        /// </returns>
        object? IOperationInfo.Invoke(object? instance, IEnumerable<object?> args)
        {
            if (!(instance is IActivity))
            {
                throw new WorkflowException($"Expected activity '{instance}' to invoke, instead received {instance?.GetType()}.");
            }

            var argsList = new List<object?> { instance };
            argsList.AddRange(args);
            var target = argsList[0];
            var arguments = (IExpando?)argsList[1];
            var context = (IActivityContext)argsList[2];
            var cancellationToken = (CancellationToken)argsList[3];

            return ExecuteAsyncMethodInfo.Call(this, argsList.ToArray());
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);

            var activityPart = this.Parts.OfType<IActivityInfo>().FirstOrDefault();
            this.ReturnType = activityPart?.ReturnType;
        }
    }
}