// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the required annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Annotations
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Behaviors;
    using Kephas.Model.Behaviors;
    using Kephas.Model.Construction;

    /// <summary>
    /// A required annotation.
    /// </summary>
    public class RequiredAnnotation : Annotation, IRequiredBehaviorRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public RequiredAnnotation(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public int ProcessingPriority { get; } = int.MinValue;

        /// <summary>
        /// Gets a value indicating whether this rule ends the processing flow.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rule ends the processing flow, <c>false</c> if not.
        /// </value>
        public bool IsEndRule { get; } = true;

        /// <summary>
        /// Gets a value asynchronously indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of a value indicating whether the rule may be applied or not.
        /// </returns>
        public Task<bool> CanApplyAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        public Task<IBehaviorValue<bool>> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult((IBehaviorValue<bool>)BehaviorValue.True);
        }

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        Task<IBehaviorValue> IAsyncBehaviorRule<IInstanceContext>.GetValueAsync(IInstanceContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult((IBehaviorValue)BehaviorValue.True);
        }
    }
}