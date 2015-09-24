// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contracts for defining asynchronous rules for behavior values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Value agnostic contract for defining an asynchronous behavior rule.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IAsyncBehaviorRule<in TContext>
    {
        /// <summary>
        /// Gets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        int ProcessingPriority { get; }

        /// <summary>
        /// Gets a value indicating whether this rule ends the processing flow.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rule ends the processing flow, <c>false</c> if not.
        /// </value>
        bool IsEndRule { get; }

        /// <summary>
        /// Gets a value asynchronously indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of a value indicating whether the rule may be applied or not.
        /// </returns>
        Task<bool> CanApplyAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        Task<IBehaviorValue> GetValueAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for defining an asynchronous behavior rule.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    /// <typeparam name="TValue">  The type of the behavior value.</typeparam>
    public interface IAsyncBehaviorRule<in TContext, TValue> : IAsyncBehaviorRule<TContext>
    {
        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        new Task<IBehaviorValue<TValue>> GetValueAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}