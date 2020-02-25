// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity type class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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
        /// An asynchronous result that yields the execute.
        /// </returns>
        public virtual Task<object> ExecuteAsync(
            IActivity activity,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            return activity.GetTypeInfo().ExecuteAsync(activity, target, arguments, context, cancellationToken);
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