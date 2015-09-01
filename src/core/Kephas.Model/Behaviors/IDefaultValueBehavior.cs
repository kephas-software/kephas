// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultValueBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Behavior for providing the default value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    /// <summary>
    /// Non-generic behavior for providing the default value.
    /// </summary>
    public interface IDefaultValueBehavior : IBehavior
    {
    }

    /// <summary>
    /// Behavior for providing the default value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IDefaultValueBehavior<TValue> : IDefaultValueBehavior, IBehavior<TValue>
    {
    }
}