// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorRuleBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for behavior rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;
using Kephas.Runtime;

namespace Kephas.Behaviors
{
    /// <summary>
    /// Base class for behavior rules.
    /// </summary>
    /// <typeparam name="TContext">Type of the context.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class BehaviorRuleBase<TContext, TValue> : BehaviorRuleFlowControlBase, IBehaviorRule<TContext, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorRuleBase{TContext, TValue}"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected BehaviorRuleBase(IRuntimeTypeRegistry typeRegistry, ILogManager? logManager = null)
            : base(typeRegistry, logManager)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A value indicating whether the rule may be applied or not.
        /// </returns>
        public virtual bool CanApply(TContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the behavior value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The behavior value.
        /// </returns>
        IBehaviorValue IBehaviorRule<TContext>.GetValue(TContext context)
        {
            return this.GetValue(context);
        }

        /// <summary>
        /// Gets the behavior value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The behavior value.
        /// </returns>
        public abstract IBehaviorValue<TValue> GetValue(TContext context);
    }
}