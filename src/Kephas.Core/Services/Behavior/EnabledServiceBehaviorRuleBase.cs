// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceBehaviorRuleBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for behavior rules controlling the enabled state of services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using Kephas.Behavior;

    /// <summary>
    /// Base class for behavior rules controlling the enabled state of services.
    /// It applyes for services implementing the contract <typeparamref name="TServiceContract"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TServiceContract> : BehaviorRuleBase<IServiceBehaviorContext<TServiceContract>, bool>, IEnabledServiceBehaviorRule<TServiceContract>
    {
    }

    /// <summary>
    /// Base class for behavior rules controlling the enabled state of the service <typeparamref name="TServiceImplementation"/>.
    /// The service must implement the contract <typeparamref name="TServiceContract"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TServiceContract, TServiceImplementation> : EnabledServiceBehaviorRuleBase<TServiceContract>
        where TServiceImplementation : TServiceContract
    {
        /// <summary>
        /// Base class for behavior rules controlling the enabled state of services 
        /// implementing the contract <typeparamref name="TServiceContract"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A value indicating whether the rule may be applied or not.
        /// </returns>
        public override bool CanApply(IServiceBehaviorContext<TServiceContract> context)
        {
            // TODO clarify whether this check will apply to the class hierarchy or to the type itself
            return context.Service?.GetType() == typeof(TServiceImplementation);
        }
    }
}