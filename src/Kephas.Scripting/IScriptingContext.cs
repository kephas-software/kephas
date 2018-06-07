// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScriptingContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScriptExecutionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Interface for scripting context.
    /// </summary>
    public interface IScriptingContext : IContext
    {
        /// <summary>
        /// Gets the script to execute.
        /// </summary>
        /// <value>
        /// The script to execute.
        /// </value>
        IScript Script { get; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IExpando Args { get; }

        /// <summary>
        /// Gets the script globals.
        /// </summary>
        /// <value>
        /// The script globals.
        /// </value>
        IScriptGlobals ScriptGlobals { get; }

        /// <summary>
        /// Gets a context for the execution.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
        IContext ExecutionContext { get; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        object Result { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; set; }
    }
}