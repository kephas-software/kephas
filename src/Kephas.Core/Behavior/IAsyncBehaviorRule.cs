// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncBehaviorRule.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contracts for defining asynchronous rules for behavior values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behavior
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Value agnostic contract for defining an asynchronous behavior rule.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IAsyncBehaviorRule<in TContext> : IBehaviorRuleFlowControl
    {
        /// <summary>
        /// Gets a value asynchronously indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of a value indicating whether the rule may be applied or not.
        /// </returns>
        Task<bool> CanApplyAsync(TContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        Task<IBehaviorValue> GetValueAsync(TContext context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Contract for defining an asynchronous behavior rule.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    /// <typeparam name="TValue">The type of the behavior value.</typeparam>
    public interface IAsyncBehaviorRule<in TContext, TValue> : IAsyncBehaviorRule<TContext>
    {
        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        new Task<IBehaviorValue<TValue>> GetValueAsync(TContext context, CancellationToken cancellationToken = default);
    }
}