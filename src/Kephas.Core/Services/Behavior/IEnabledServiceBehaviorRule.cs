// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnabledServiceBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Behavior rule contract for controlling the enabled state of services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using Kephas.Behavior;

    /// <summary>
    /// Interface for enabled service behavior rule.
    /// </summary>
    public interface IEnabledServiceBehaviorRule : IBehaviorRule<IContext, bool>
    {
    }

    /// <summary>
    /// Behavior rule contract for controlling the enabled state of services.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    [SharedAppServiceContract(ContractType = typeof(IEnabledServiceBehaviorRule), AllowMultiple = true)]
    public interface IEnabledServiceBehaviorRule<in TServiceContract> : IBehaviorRule<IServiceBehaviorContext<TServiceContract>, bool>, IEnabledServiceBehaviorRule
    {
    }
}