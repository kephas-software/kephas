// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptingContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting;

using System.Diagnostics.CodeAnalysis;
using Kephas.Services;

/// <summary>
/// Extension methods for <see cref="IScriptingContext"/>.
/// </summary>
public static class ScriptingContextExtensions
{
    /// <summary>
    /// Sets a value indicating whether the <see cref="IScriptingContext.Args" /> should be deconstructed.
    /// If <c>true</c>, the values in <see cref="IScriptingContext.Args" /> are globally available by their name,
    /// otherwise the arguments are available through the global Args value.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="value"><c>true</c> to deconstruct the arguments, <c>false</c> otherwise.</param>
    /// <returns>The provided context.</returns>
    [return: NotNull]
    public static T DeconstructArgs<T>([DisallowNull] this T context, bool value)
        where T : IScriptingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        context.DeconstructArgs = value;
        return context;
    }

    /// <summary>
    /// Sets the script execution context.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <typeparam name="TExecution">The execution context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="value"><c>true</c> to deconstruct the arguments, <c>false</c> otherwise.</param>
    /// <returns>The provided context.</returns>
    [return: NotNull]
    public static T ExecutionContext<T, TExecution>([DisallowNull] this T context, TExecution value)
        where T : IScriptingContext
        where TExecution : IContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        context.ExecutionContext = value;
        return context;
    }

    /// <summary>
    /// Adds a new global variable to the script.
    /// If the global variable already exists, it will be overwritten.
    /// </summary>
    /// <typeparam name="T">The context type.</typeparam>
    /// <param name="context">The context.</param>
    /// <param name="name">The global variable name.</param>
    /// <param name="value">The global variable value.</param>
    /// <returns>
    /// The provided context.
    /// </returns>
    [return: NotNull]
    public static T AddGlobal<T>([DisallowNull] this T context, string name, object? value)
        where T : IScriptingContext
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        context.ScriptGlobals[name] = value;
        return context;
    }
}