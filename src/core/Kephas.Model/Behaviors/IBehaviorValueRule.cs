// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBehaviorValueRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contracts for defining rules for behavior values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;

    /// <summary>
    /// Non-generic contract for defining a rule for behavior values.
    /// </summary>
    public interface IBehaviorValueRule
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
        Task<bool> CanApplyAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        Task<object> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for defining a rule for behavior values.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IBehaviorValueRule<TValue> : IBehaviorValueRule
    {
        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        new Task<TValue> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}