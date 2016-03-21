// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncModelBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for asynchronous behavior rules in model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using Kephas.Behavior;
    using Kephas.Data;

    /// <summary>
    /// Non-generic contract for asynchronous behavior rules in model.
    /// </summary>
    public interface IAsyncModelBehaviorRule : IAsyncBehaviorRule<IInstanceContext>
    {
    }

    /// <summary>
    /// Contract for asynchronous behavior rules in model.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IAsyncModelBehaviorRule<TValue> : IAsyncModelBehaviorRule, IAsyncBehaviorRule<IInstanceContext, TValue>
    {
    }
}