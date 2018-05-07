// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using Kephas.Dynamic;

    /// <summary>
    /// Base contract for activities.
    /// </summary>
    /// <remarks>
    /// The values in the expando are the activity arguments.
    /// </remarks>
    public interface IActivity : IExpando, IInstance
    {
    }
}