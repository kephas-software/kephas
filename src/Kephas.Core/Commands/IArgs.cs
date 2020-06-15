// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application arguments interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for accessing arguments.
    /// </summary>
    public interface IArgs : IExpando
    {
        /// <summary>
        /// Converts this app arguments list to a list of string arguments for use in command lines.
        /// </summary>
        /// <returns>A list of string arguments.</returns>
        IEnumerable<string> ToCommandArgs();
    }
}
