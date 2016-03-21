// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorRuleBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for behavior rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behavior
{
    /// <summary>
    /// Base class for behavior rules.
    /// </summary>
    /// <typeparam name="TContext">Type of the context.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class BehaviorRuleBase<TContext, TValue> : BehaviorRuleFlowControlBase, IBehaviorRule<TContext, TValue>
    {
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