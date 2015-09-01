// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for behaviors attached to model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;

    /// <summary>
    /// Contract for behaviors attached to model elements.
    /// </summary>
    /// <remarks>
    /// Behaviors are a special kind of model elements which provide a value in a certain context. 
    /// This value used in conjuction with the behavior type may influence the runtime behavior of instances.
    /// </remarks>
    public interface IBehavior
    {
        /// <summary>
        /// Gets the value of the provided behavior.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The value.
        /// </returns>
        Task<object> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for typed behaviors attached to model elements.
    /// </summary>
    /// <typeparam name="TValue">The type of the behavior value.</typeparam>
    public interface IBehavior<TValue> : IBehavior
    {
        /// <summary>
        /// Gets the value of the provided behavior.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The value.
        /// </returns>
        new Task<TValue> GetValueAsync(IInstanceContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}