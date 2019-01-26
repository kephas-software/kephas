// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivityInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Primary metadata for an activity.
    /// </summary>
    public interface IActivityInfo : ITypeInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        IEnumerable<IParameterInfo> Parameters { get; }
    }
}