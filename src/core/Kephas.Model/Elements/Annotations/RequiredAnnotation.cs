namespace Kephas.Model.Elements.Annotations
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Model.Behaviors;
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// A required annotation.
    /// </summary>
    public class RequiredAnnotation : Annotation, IRequiredBehaviorRule
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Kephas.Model.Elements.Annotations.RequiredAnnotation"/> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <param name="modelSpace"> The model space.</param>
        public RequiredAnnotation(IAnnotationInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
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
        public Task<bool> CanApplyAsync(IInstanceContext context, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<bool> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken)
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
        Task<object> IBehaviorRule.GetValueAsync(IInstanceContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult((object)true);
        }
    }
}