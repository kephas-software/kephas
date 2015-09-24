// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultValueBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Non-generic value rule contract for the behavior providing the default value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    /// <summary>
    /// Non-generic value rule contract for the behavior providing the default value.
    /// </summary>
    public interface IDefaultValueBehaviorRule : IAsyncModelBehaviorRule
    {
    }

    /// <summary>
    /// Value rule contract for the behavior providing the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the default value.</typeparam>
    public interface IDefaultValueBehaviorRule<TValue> : IDefaultValueBehaviorRule, IAsyncModelBehaviorRule<TValue>
    {
    }
}